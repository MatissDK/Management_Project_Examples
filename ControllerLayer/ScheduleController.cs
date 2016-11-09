namespace ControllerLayer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using ModuleLayer;

    public class ScheduleController
    {
        #region Constructors and Destructors

        public ScheduleController()
        {
            this.Appointments = new List<Appointment>();
            this.WorkingDays = new List<Day>();
        }

        #endregion

        #region Public Properties

        public List<Appointment> Appointments { get; private set; }

        public List<Day> WorkingDays { get; private set; }

        #endregion

        #region Public Methods and Operators

        public int AddAppointment(DateTime currentDate, string timeFrom, string timeTo, string clientName, string clientPhone, Employee employee, string serviceType)
        {
            if (this.Appointments.Any(e => e.ClientName == clientName && e.Date == currentDate && e.Employee.Equals(employee) && e.ServiceType == serviceType && e.TimeFrom.Equals(timeFrom) && e.TimeTo.Equals(timeTo)))
                return -2;

            var paramsList = new List<SqlParameter> { new SqlParameter("@Date", currentDate), new SqlParameter("@TimeFrom", timeFrom), new SqlParameter("@TimeTo", timeTo), new SqlParameter("@ClientName", clientName), new SqlParameter("@ClientPhone", clientPhone), new SqlParameter("@EmployeeId", employee.Id), new SqlParameter("@ServiceType", serviceType) };
            const string procedureName = "AddAppointment";

            int returnResult = new DBController().RunProcedureInDatabase(procedureName, "EJL15_DB", paramsList);

            if (returnResult != int.MinValue)
                this.Appointments.Add(new Appointment(returnResult, clientName, clientPhone, employee, timeFrom, timeTo, serviceType, currentDate));

            return returnResult;
        }

        public int ManageEmployeeInDay(DateTime currentDate, Employee employee, bool deleteAction)
        {
            if (!deleteAction)
            {
                if ((from curDay in this.WorkingDays where curDay.Date.Equals(currentDate) select curDay.Employees.FindIndex(e => e.Id == employee.Id)).Any(index => index >= 0))
                    return -2;
            }

            var paramsList = new List<SqlParameter> { new SqlParameter("@Date", currentDate), new SqlParameter("@EmployeeId", employee.Id) };

            string procedureName = "AddEmployeeToDay";

            if (deleteAction)
                procedureName = "DeleteEmployeeFromDay";

            int returnResult = new DBController().RunProcedureInDatabase(procedureName, "EJL15_DB", paramsList);

            if (returnResult == int.MinValue && !deleteAction)
                return returnResult;

            foreach (Day curDay in this.WorkingDays.Where(curDay => curDay.Date.Equals(currentDate)))
            {
                if (deleteAction)
                    curDay.Employees.Remove(employee);
                curDay.Employees.Add(employee);
            }

            return returnResult;
        }

        public void PopulateAppointmentList(List<Employee> employeeList)
        {
            var paramsList = new List<SqlParameter>();
            const string procedureName = "RetrieveAppointment";
            this.Appointments = new DBController().GetFromDatabase(procedureName, "Appointment", "EJL15_DB", paramsList, employeeList).Cast<Appointment>().ToList();
        }

        public void PopulateWorkingDaysList(List<Employee> employeeList)
        {
            var paramsList = new List<SqlParameter>();
            object curObject = new List<Object> { this.Appointments, employeeList };

            // TODO Implement query for appointments
            const string procedureName = "RetrieveDay";
            this.WorkingDays = new DBController().GetFromDatabase(procedureName, "Day", "EJL15_DB", paramsList, curObject).Cast<Day>().ToList();
        }

        #endregion
    }
}