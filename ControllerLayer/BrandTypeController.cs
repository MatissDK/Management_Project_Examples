namespace ControllerLayer
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;

    using ModuleLayer;

    public class BrandTypeController
    {
        #region Constructors and Destructors

        public BrandTypeController()
        {
            this.Types = new List<Type>();
            this.Brands = new List<Brand>();
        }

        #endregion

        #region Public Properties

        public List<Brand> Brands { get; set; }

        public List<Type> Types { get; set; }

        #endregion

        #region Public Methods and Operators

        public int AddToBrand(string brandName)
        {
            if (this.Brands.Any(e => e.BrandName == brandName))
                return -2;

            var paramsList = new List<SqlParameter> { new SqlParameter("@Brand", brandName) };
            const string ProcedureName = "AddBrand";
            int returnResult = new DBController().RunProcedureInDatabase(ProcedureName, "EJL15_DB", paramsList);

            if (returnResult != int.MinValue)
                this.Brands.Add(new Brand(returnResult, brandName));

            return returnResult;
        }

        public int AddToType(string typeName)
        {
            if (this.Types.Any(e => e.TypeName == typeName))
                return -2;

            var paramsList = new List<SqlParameter> { new SqlParameter("@Type", typeName) };

            const string ProcedureName = "AddTypes";
            int returnResult = new DBController().RunProcedureInDatabase(ProcedureName, "EJL15_DB", paramsList);

            if (returnResult != int.MinValue)
                this.Types.Add(new Type(returnResult, typeName));

            return returnResult;
        }

        public void DeleteBrand(int id)
        {
            var paramsList = new List<SqlParameter> { new SqlParameter("@Id", id) };
            const string procedureName = "DeleteBrand";
            new DBController().RunProcedureInDatabase(procedureName, "EJL15_DB", paramsList);
        }

        public void DeleteType(int id)
        {
            var paramsList = new List<SqlParameter> { new SqlParameter("@Id", id) };
            const string procedureName = "DeleteType";
            new DBController().RunProcedureInDatabase(procedureName, "EJL15_DB", paramsList);
        }

        public Brand GetBrandByName(string brandName)
        {
            return this.Brands.Find(e => e.BrandName == brandName);
        }

        public Type GetTypeByName(string typeName)
        {
            return this.Types.Find(e => e.TypeName == typeName);
        }

        public void PopulateBrandList()
        {
            const string procedureName = "RetrieveBrand";
            this.Brands = new DBController().GetFromDatabase(procedureName, "Brand", "EJL15_DB", new List<SqlParameter>()).Cast<Brand>().ToList();
        }

        public void PopulateTypeList()
        {
            const string procedureName = "RetrieveType";
            this.Types = new DBController().GetFromDatabase(procedureName, "Type", "EJL15_DB", new List<SqlParameter>()).Cast<Type>().ToList();
        }

        #endregion
    }
}