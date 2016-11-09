namespace ControllerLayer
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using ModuleLayer;

    public class ProductController
    {
        #region Constants

        private const int Quantity = 0;

        #endregion

        #region Constructors and Destructors

        public ProductController()
        {
            this.Products = new List<Product>();
        }

        #endregion

        #region Public Properties

        public List<Product> Products { get; set; }

        #endregion

        #region Public Methods and Operators

        public int AddProduct(Brand brand, Type type, string name, int volume)
        {
            if (this.Products.Any(e => e.Name.Equals(name) && e.Brand.BrandName.Equals(brand.BrandName) && e.Type.TypeName.Equals(type.TypeName) && e.Volume.Equals(volume)))
                return -2;
            

            var paramsList = new List<SqlParameter> { new SqlParameter("@Name", name), new SqlParameter("@Volume", volume), new SqlParameter("@Brand", brand.Id), new SqlParameter("@Type", type.Id) };

            const string procedureName = "AddProduct";
            int returnResult = RunProcedure(procedureName, paramsList); // TODO Hardcoded catalog name. Do something about it later.

            if (returnResult != int.MinValue)
                this.Products.Add(new Product(returnResult, brand, type, name, volume, Quantity));

            return returnResult;
        }

        public void PopulateProductList()
        {
            const string procedureName = "RetrieveProduct";
            this.Products = new DBController().GetFromDatabase(procedureName, "Product", "EJL15_DB", new List<SqlParameter>()).Cast<Product>().ToList();
        }

        public int UpdateDepedencies(int selectedQuantity, int productId)
        {
            var paramsList = new List<SqlParameter> { new SqlParameter("@Quantity", selectedQuantity), new SqlParameter("@ProductId", productId) };

            const string ProcedureName = "UpdateProductDependencies";
            int returnResult = RunProcedure(ProcedureName, paramsList);

            if (returnResult != int.MinValue)
                this.Products.Find(i => i.Id == productId).Quantity += selectedQuantity;

            return returnResult;
        }

        #endregion

        #region Methods

        private static int RunProcedure(string procedureName, IEnumerable<SqlParameter> paramsList)
        {
            return new DBController().RunProcedureInDatabase(procedureName, "EJL15_DB", paramsList);
        }

        #endregion
    }
}