namespace ControllerLayer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using ModuleLayer;

    using Type = ModuleLayer.Type;

    public class DBController
    {
        // TODO Change text queries to Stored Procedures (more efficient way)

        #region Fields

        private SqlCommand sqlCommand;

        private SqlConnection sqlConnection;

        private SqlDataReader sqlReader;

        #endregion

        #region Public Methods and Operators

        public List<Object> GetFromDatabase(string procedureName, string curObject, string catalog, IEnumerable<SqlParameter> paramsList, object additionalObject)
        {
            try
            {
                this.ReaderInitializer(procedureName, catalog, paramsList);
                return this.ReadData(curObject, additionalObject);
            }
            catch (Exception e)
            {
                throw e.GetBaseException();
            }
            finally
            {
                this.DisposeUnmanagedResources();
            }
        }

        public List<Object> GetFromDatabase(string procedureName, string curObject, string catalog, IEnumerable<SqlParameter> paramsList)
        {
            try
            {
                this.ReaderInitializer(procedureName, catalog, paramsList);
                return this.ReadData(curObject);
            }
            catch (Exception e)
            {
                throw e.GetBaseException();
            }
            finally
            {
                this.DisposeUnmanagedResources();
            }
        }

        public List<Object> ReadData(string curObject, object additionalObject)
        {
            var populatedList = new List<Object>();

            while (this.sqlReader.Read())
            {
                List<Employee> employeeList;
                switch (curObject)
                {
                    case "Stock":
                        employeeList = additionalObject as List<Employee>;
                        populatedList.Add(this.StockRow(employeeList));
                        break;
                    case "Appointment":
                        employeeList = additionalObject as List<Employee>;
                        populatedList.Add(this.AppointmentRow(employeeList));
                        break;
                    case "Day":
                        var appointmentList = new List<Appointment>();
                        employeeList = new List<Employee>();
                        var curObjects = additionalObject as List<Object>;

                        if (curObjects != null)
                        {
                            appointmentList = curObjects[0] as List<Appointment>;
                            employeeList = curObjects[1] as List<Employee>;
                        }
                        populatedList.Add(this.DayRow(appointmentList, employeeList));
                        break;
                }
            }

            return populatedList;
        }

        public List<Object> ReadData(string curObject)
        {
            var populatedList = new List<Object>();

            while (this.sqlReader.Read())
            {
                var curSpecializationList = new List<Specialization>();
                switch (curObject)
                {
                    case "Brand":
                        populatedList.Add(this.BrandRow());
                        break;
                    case "Type":
                        populatedList.Add(this.TypeRow());
                        break;
                    case "Product":
                        populatedList.Add(this.ProductRow());
                        break;
                    case "Login":
                        populatedList.Add(this.LoginRow());
                        break;
                    case "Employee":
                        //curSpecializationList.Add(SpecializationRow());
                        //populatedList.Add(this.EmployeeRow(additionalObject as List<Specialization>));
                        populatedList.Add(this.EmployeeRow(curSpecializationList));
                        break;
                }
            }

            return populatedList;
        }

        public int RunProcedureInDatabase(string procedureName, string catalog, IEnumerable<SqlParameter> paramsList)
        {
            try
            {
                this.CreateNewConnection(catalog);
                this.SetCommandProperties(procedureName, paramsList);

                return (int?) this.sqlCommand.ExecuteScalar() ?? -1;
            }
            catch (Exception)
            {
                return int.MinValue;
            }
            finally
            {
                this.DisposeUnmanagedResources();
            }
        }

        #endregion

        #region Methods

        internal void DisposeUnmanagedResources()
        {
            this.sqlConnection.Close();
            this.sqlConnection.Dispose();
            this.sqlCommand.Dispose();
            if (this.sqlReader != null)
                this.sqlReader.Dispose();
        }

        private static string CreateConnectionString(string catalog)
        {
            var builder = new SqlConnectionStringBuilder();
            builder["Data Source"] = "ealdb1.eal.local";
            builder["integrated Security"] = false;
            builder["Initial Catalog"] = catalog;
            builder["User ID"] = "ejl15_usr";
            builder["Password"] = "Baz1nga15";
            builder["Connection Timeout"] = 3;
            return builder.ConnectionString;
        }

        private Appointment AppointmentRow(List<Employee> employeeList)
        {
            Employee curEmployee = employeeList.Find(e => e.Id == (int) this.sqlReader["aEmployeeId"]);
            return new Appointment((int) this.sqlReader["aId"], this.sqlReader["aClientName"].ToString(), 
                this.sqlReader["aClientPhone"].ToString(), curEmployee, DateTime.Parse(this.sqlReader["aFrom"].ToString()).
                ToString("HH:mm"), DateTime.Parse(this.sqlReader["aTo"].ToString()).ToString("HH:mm"), this.sqlReader["aService"].
                ToString(), DateTime.Parse(this.sqlReader["aDate"].ToString()));
        }

        private Brand BrandRow()
        {
            return new Brand((int) this.sqlReader["bId"], this.sqlReader["bName"].ToString());
        }

        private void CreateNewConnection(string catalog)
        {
            this.sqlConnection = new SqlConnection { ConnectionString = CreateConnectionString(catalog) };

            this.sqlConnection.Open();

            this.sqlCommand = this.sqlConnection.CreateCommand();
        }

        private Day DayRow(List<Appointment> appointmentList, IEnumerable<Employee> employeeList)
        {
            string curDate = DateTime.Parse(this.sqlReader["dDate"].ToString()).ToString("dd.MM.yy");
            appointmentList = appointmentList.FindAll(e => e.Date.ToString("dd.MM.yy") == curDate);
            var employees = new List<Employee>();

            foreach (Appointment curAppointments in appointmentList.Where(curAppointments => !employees.Contains(curAppointments.Employee)))
                employees.Add(curAppointments.Employee);

            foreach (Employee curEmployee in employeeList.Where(curEmployee => !employees.Contains(curEmployee) 
                && curEmployee.Id == (int) this.sqlReader["dEmployeeId"]))
                employees.Add(curEmployee);

            return new Day(DateTime.Parse(curDate), employees, appointmentList);
        }

        private Employee EmployeeRow(List<Specialization> curList)
        {
            return new Employee((int) this.sqlReader["eId"], this.sqlReader["eName"].ToString(), 
                this.sqlReader["eSurName"].ToString(), this.sqlReader["ePhoneNumber"].ToString(), 
                this.sqlReader["eEmail"].ToString(), this.sqlReader["eAddress"].ToString(), curList, this.sqlReader["eZipCode"].ToString());
        }

        private Login LoginRow()
        {
            return new Login((int) this.sqlReader["orderId"], this.sqlReader["name"].ToString(), this.sqlReader["surname"].ToString());
        }

        private Product ProductRow()
        {
            return new Product((int) this.sqlReader["pId"], this.BrandRow(), this.TypeRow(), 
                this.sqlReader["pName"].ToString(), (int) this.sqlReader["pVolume"], (int) this.sqlReader["pQuantity"]);
        }

        private void ReaderInitializer(string procedureName, string catalog, IEnumerable<SqlParameter> paramsList)
        {
            this.CreateNewConnection(catalog);
            this.SetCommandProperties(procedureName, paramsList);

            this.sqlReader = this.sqlCommand.ExecuteReader();
        }

        private void SetCommandProperties(string procedureName, IEnumerable<SqlParameter> paramsList)
        {
            this.sqlCommand.CommandText = procedureName;
            this.sqlCommand.CommandType = CommandType.StoredProcedure;
            foreach (SqlParameter curParam in paramsList)
                this.sqlCommand.Parameters.Add(curParam);

            this.sqlCommand.CommandTimeout = 15;
        }

        private Specialization SpecializationRow()
        {
            return new Specialization((int) this.sqlReader["specId"], this.sqlReader["specName"].ToString());
        }

        private Stock StockRow(List<Employee> employeeList)
        {
            Employee curEmployee = employeeList.Find(e => e.Id == (int) this.sqlReader["sEmployeeId"]);
            string curDate = this.sqlReader["sDate"].ToString();
            return new Stock(DateTime.Parse(curDate), this.ProductRow(), (int) this.sqlReader["sQuantity"], 
                Convert.ToChar(this.sqlReader["sType"].ToString()), curEmployee);
        }

        private Type TypeRow()
        {
            return new Type((int) this.sqlReader["tId"], this.sqlReader["tName"].ToString());
        }

        #endregion
    }
}