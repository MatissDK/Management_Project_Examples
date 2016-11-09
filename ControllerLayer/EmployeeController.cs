namespace ControllerLayer
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using ModuleLayer;

    public class EmployeeController
    {
        #region Constructors and Destructors

        public EmployeeController()
        {
            this.EmployeeList = new List<Employee>();
        }

        #endregion

        #region Public Properties

        public List<Employee> EmployeeList { get; private set; }

        #endregion

        #region Public Methods and Operators

        public int AddEmployee(string name, string surname, string telNumber, string address, string zipCode, string email, string user, string password, List<string> specializationStringList)
        {
            if (this.EmployeeList.Any(e => e.Name == name && e.SurName == surname))
                return -2;

            var paramsList = new List<SqlParameter> { new SqlParameter("@Name", name), new SqlParameter("@Surname", surname), new SqlParameter("@TelNumber", telNumber), new SqlParameter("@Address", address), new SqlParameter("@ZipCode", zipCode), new SqlParameter("@Email", email) };

            string procedureName = "AddEmployee";
            int returnResult = new DBController().RunProcedureInDatabase(procedureName, "EJL15_DB", paramsList);
            paramsList.Clear();

            if (returnResult == -1 && returnResult == int.MinValue)
                return returnResult;

            foreach (string specializationId in specializationStringList)
            {
                paramsList = new List<SqlParameter> { new SqlParameter("@EmployeeId", returnResult), new SqlParameter("@SpecializationId", specializationId) };
                procedureName = "AddEmployeeSpecialization";
                new DBController().RunProcedureInDatabase(procedureName, "EJL15_DB", paramsList);
                paramsList.Clear();
            }

            paramsList.Clear();
            paramsList = new List<SqlParameter> { new SqlParameter("@OrderId", returnResult), new SqlParameter("@BatchName", user), new SqlParameter("@ProductCode", password) };

            procedureName = "AddOrder";
            int usenameAndPasswordQueryResult = new DBController().RunProcedureInDatabase(procedureName, "EJL15_DB", paramsList);

            if (usenameAndPasswordQueryResult != -1)
                this.EmployeeList.Add(new Employee(returnResult, name, surname, telNumber, address, email, new List<Specialization>(), zipCode));

            return returnResult;
        }

        public void PopulateEmployeeList()
        {
            const string ProcedureName = "RetrieveEmployee";
            this.EmployeeList = new DBController().GetFromDatabase(ProcedureName, "Employee", "EJL15_DB", new List<SqlParameter>()).Cast<Employee>().ToList();
        }

        #endregion
    }
}