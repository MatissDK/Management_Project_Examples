#region

#endregion

namespace ModuleLayer
{
    using System;
    using System.Collections.Generic;

    public class Day
    {
        #region Constructors and Destructors

        public Day(DateTime date, List<Employee> employees, List<Appointment> appointments)
        {
            this.Date = date;
            this.Employees = employees;
            this.Appointments = appointments;
        }

        #endregion

        #region Public Properties

        public List<Appointment> Appointments { get; private set; }

        public DateTime Date { get; private set; }

        public List<Employee> Employees { get; private set; }

        #endregion
    }
}