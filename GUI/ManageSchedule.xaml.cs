namespace GUI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    using ControllerLayer;

    using DelegateArgsLayer;

    using ModuleLayer;

    /// <summary>
    ///     Interaction logic for ManageSchedule.xaml
    /// </summary>
    public partial class ManageSchedule : Window
    {
        #region Fields

        private readonly List<Employee> activeEmployeeList = new List<Employee>();

        private readonly List<Employee> allEmployeesList = new List<Employee>();

        private readonly List<Employee> oldActiveEmployeeList = new List<Employee>();

        private readonly ScheduleController scheduleController;

        private DateTime currentDate;

        private ListBox dragSource;

        #endregion

        #region Constructors and Destructors

        public ManageSchedule(ScheduleController scheduleController, IEnumerable<Employee> allEmployeeList, IEnumerable scEmployeeItems, string curDate)
        {
            this.scheduleController = scheduleController;
            try
            {
                this.currentDate = DateTime.Parse(curDate);
            }
            catch {}

            this.InitializeComponent();

            foreach (object curEmployee in scEmployeeItems)
            {
                this.activeEmployeeList.Add(curEmployee as Employee);
                this.oldActiveEmployeeList.Add(curEmployee as Employee);
                this.ScActiveEmployeeGrid.Items.Add(curEmployee);
            }

            foreach (Employee curEmployee in allEmployeeList.Where(curEmployee => !this.ScActiveEmployeeGrid.Items.Contains(curEmployee)))
            {
                this.allEmployeesList.Add(curEmployee);
                this.ScAllEmployeeGrid.Items.Add(curEmployee);
            }

            this.ScMeDateLabel.Content = curDate;
        }

        #endregion

        #region Delegates

        public delegate void ManageScheduleHander(object source, DelegateEventArgs e);

        #endregion

        #region Public Events

        public static event ManageScheduleHander UpdateScheduleListEvent;

        #endregion

        #region Methods

        private static object GetData(ItemsControl curGrid, Point point)
        {
            var element = curGrid.InputHitTest(point) as UIElement;
            if (element == null)
                return null;

            object data = DependencyProperty.UnsetValue;

            while (data == DependencyProperty.UnsetValue)
            {
                data = curGrid.ItemContainerGenerator.ItemFromContainer(element);
                if (data == DependencyProperty.UnsetValue)
                {
                    if (element != null)
                        element = VisualTreeHelper.GetParent(element) as UIElement;
                }
                else
                    return data;

                if (element != null && element.Equals(curGrid))
                    return null;
            }
            return null;
        }

        private void DataGrid_Drop(object sender, DragEventArgs e)
        {
            var curGrid = sender as ListBox;

            if (curGrid != null && curGrid.Equals(this.dragSource))
                return;

            object data = e.Data.GetData(typeof(Employee));
            var curList = this.dragSource.Items as IList;
            if (curList != null)
                curList.Remove(data);

            if (curGrid != null)
                curGrid.Items.Add(data);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            this.ScActiveEmployeeGrid.Items.Clear();
            foreach (Employee curEmployee in this.activeEmployeeList)
                this.ScActiveEmployeeGrid.Items.Add(curEmployee);

            this.ScAllEmployeeGrid.Items.Clear();
            foreach (Employee curEmployee in this.allEmployeesList)
                this.ScAllEmployeeGrid.Items.Add(curEmployee);
        }

        private void ScAllEmployeeGrid_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var curGrid = sender as ListBox;
            this.dragSource = curGrid;

            if (curGrid == null)
                return;

            object data = GetData(this.dragSource, e.GetPosition(curGrid));

            if (data == null)
                return;

            DragDrop.DoDragDrop(curGrid, data, DragDropEffects.Move);
        }

        private void ScMeCancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ScMeSaveButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (Employee curEmployee in this.ScActiveEmployeeGrid.Items)
            {
                if (this.oldActiveEmployeeList.Contains(curEmployee))
                    this.oldActiveEmployeeList.Remove(curEmployee);
                this.scheduleController.ManageEmployeeInDay(this.currentDate, curEmployee, false);
            }

            foreach (Employee curEmployee in this.oldActiveEmployeeList)
                this.scheduleController.ManageEmployeeInDay(this.currentDate, curEmployee, true);

            var a = new DelegateEventArgs(this.currentDate, true);
            UpdateScheduleListEvent(this, a);

            MessageBox.Show("Employees for " + this.currentDate.ToString("dd.MM.yy") + " were saved.");
            this.Close();
        }

        #endregion
    }
}