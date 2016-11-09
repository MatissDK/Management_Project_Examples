namespace GUI
{
    using System.Collections.Generic;
    using System.Windows;

    using DelegateArgsLayer;

    /// <summary>
    ///     Interaction logic for ChooseSpecialization.xaml
    /// </summary>
    public partial class ChooseSpecialization : Window
    {
        #region Constructors and Destructors

        public ChooseSpecialization()
        {
            this.InitializeComponent();
        }

        #endregion

        #region Delegates

        public delegate void ChooseSpecializatiotHandler(object source, DelegateEventArgs e);

        #endregion

        #region Public Events

        public static event ChooseSpecializatiotHandler AddVariables;

        #endregion

        #region Methods

        private void AddVariablesMethod(List<string> specializationStringList)
        {
            var e = new DelegateEventArgs(specializationStringList);
            if (AddVariables != null)
                AddVariables(this, e);
        }

        private void SpecializationCancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SpecializationOKButton_Click(object sender, RoutedEventArgs e)
        {
            var specializationStringList = new List<string>();
            if (this.Hairdresser.IsChecked == true)
                specializationStringList.Add("1");
            if (this.Cosmetologist.IsChecked == true)
                specializationStringList.Add("2");
            if (this.Manicurist.IsChecked == true)
                specializationStringList.Add("3");
            if (this.Administrator.IsChecked == true)
                specializationStringList.Add("4");

            this.AddVariablesMethod(specializationStringList);

            this.Close();
        }

        #endregion
    }
}