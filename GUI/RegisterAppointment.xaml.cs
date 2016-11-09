namespace GUI
{
    using System;
    using System.Windows;

    using ControllerLayer;

    using DelegateArgsLayer;

    using ModuleLayer;

    /// <summary>
    ///     Interaction logic for RegisterAppointment.xaml
    /// </summary>
    public partial class RegisterAppointment : Window
    {
        #region Fields

        private readonly DateTime currentDate;

        private readonly Employee currentEmployee;

        private readonly ScheduleController scheduleController;

        #endregion

        #region Constructors and Destructors

        public RegisterAppointment(DateTime currentDate, Employee currentEmployee, ScheduleController scheduleController)
        {
            this.scheduleController = scheduleController;
            this.currentDate = currentDate;
            this.currentEmployee = currentEmployee;
            this.InitializeComponent();
            this.RaCurrentDateLabel.Content = this.currentDate.ToString("dd.MM.yy");
            this.RaEmployeeTextbox.Text = this.currentEmployee.ToString();
        }

        #endregion

        #region Delegates

        public delegate void RegisterAppointmentHander(object source, DelegateEventArgs e);

        #endregion

        #region Public Events

        public static event RegisterAppointmentHander UpdateAppointmentListEvent;

        #endregion

        #region Methods

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void RegisterAppointmentSave_Button(object sender, RoutedEventArgs e)
        {
            if (this.RaClientNameTextbox.Text == "" || this.RaJobTypeTextBox.Text == "" || !this.RaFromTimePicker.Value.HasValue || !this.RaToTimePicker.Value.HasValue)
            {
                MessageBox.Show("The appointment is not created. Some field are missing or you didn't provide a proper information.\r\nPlease, check if you filled all the field and the information is correct before creating an appointment again.", "Appointment creation error");
                return;
            }

            try
            {
                string timeFrom = ((DateTime) this.RaFromTimePicker.Value).ToString("HH:mm");
                string timeTo = ((DateTime) this.RaToTimePicker.Value).ToString("HH:mm");
                ;
                int returnResult = this.scheduleController.AddAppointment(this.currentDate, timeFrom, timeTo, this.RaClientNameTextbox.Text, this.RaClientPhoneTextbox.Text, this.currentEmployee, this.RaJobTypeTextBox.Text);

                if (returnResult == -2)
                    MessageBox.Show("There are already such appointment in the system.", "Appointments already exists");

                if (returnResult == int.MinValue)
                    throw new Exception();

                var a = new DelegateEventArgs(this.currentDate, false);
                UpdateAppointmentListEvent(this, a);

                MessageBox.Show("Appointment has been successfully created and added to the database.");
                this.Close();
            }
            catch (FormatException)
            {
                MessageBox.Show("There was a problem while trying to parse current time.\r\nCheck \"Time from\" and \"Time to\" fields before creating an appointment again.", "Time parse error");
            }
            catch
            {
                MessageBox.Show("Unknows error accured while trying to create an appointments in the database.\r\nCheck your internet/database connection and try again.", "Database error");
            }
        }

        #endregion
    }
}