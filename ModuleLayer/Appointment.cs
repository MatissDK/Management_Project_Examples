namespace ModuleLayer
{
    using System;

    public class Appointment
    {
        #region Constructors and Destructors

        public Appointment(int id, string clientName, string clientPhone, Employee employee, string timeFrom, string timeTo, string serviceType, DateTime date)
        {
            this.Id = id;
            this.ClientName = clientName;
            this.ClientPhone = clientPhone;
            this.Employee = employee;
            this.TimeFrom = timeFrom;
            this.TimeTo = timeTo;
            this.ServiceType = serviceType;
            this.Date = date;
        }

        #endregion

        #region Public Properties

        public string ClientName { get; private set; }

        public string ClientPhone { get; private set; }

        public DateTime Date { get; private set; }

        public Employee Employee { get; private set; }

        public int Id { get; private set; }

        public string ServiceType { get; private set; }

        public string TimeFrom { get; private set; }

        public string TimeTo { get; private set; }

        #endregion
    }
}