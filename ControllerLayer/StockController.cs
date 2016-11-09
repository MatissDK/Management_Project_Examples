namespace ControllerLayer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using ModuleLayer;

    public class StockController
    {
        #region Constructors and Destructors

        public StockController()
        {
            this.Stock = new List<Stock>();
        }

        #endregion

        #region Public Properties

        public List<Stock> Stock { get; private set; }

        #endregion

        #region Public Methods and Operators

        public int AddToStock(DateTime currentDate, Employee currentEmployee, Product selectedProduct, int selectedQuantity, char selectedType)
        {
            var paramsList = new List<SqlParameter> { new SqlParameter("@Date", currentDate), new SqlParameter("@ProductId", selectedProduct.Id), new SqlParameter("@Quantity", selectedQuantity), new SqlParameter("@EmployeeId", currentEmployee.Id), new SqlParameter("@Type", selectedType) };

            const string procedureName = "AddStock";
            int returnResult = RunProcedure(procedureName, paramsList);

            if (returnResult != int.MinValue)
                this.Stock.Add(new Stock(currentDate, selectedProduct, selectedQuantity, 'r', currentEmployee));

            return returnResult;
        }

        public int DeleteProduct()
        {
            // TODO Implement deleting and dependency check
            return 0;
        }

        public void PopulateStockList(List<Employee> employeeList)
        {
            var paramsList = new List<SqlParameter>();
            const string procedureName = "RetrieveStock";
            this.Stock = new DBController().GetFromDatabase(procedureName, "Stock", "EJL15_DB", paramsList, employeeList).Cast<Stock>().ToList();
        }

        #endregion

        #region Methods

        private static int RunProcedure(string procedureName, IEnumerable<SqlParameter> paramsList)
        {
            return new DBController().RunProcedureInDatabase(procedureName, "EJL15_DB", paramsList);
            // TODO Hardcoded catalog name. Do something about it later!!!!!!!!!
        }

        #endregion
    }
}