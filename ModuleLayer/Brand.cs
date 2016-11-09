#region

#endregion

namespace ModuleLayer
{
    public sealed class Brand
    {
        #region Constructors and Destructors

        public Brand(int id, string brandName)
        {
            this.Id = id;
            this.BrandName = brandName;
        }

        #endregion

        #region Public Properties

        public string BrandName { get; private set; }

        public int Id { get; private set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return this.BrandName;
        }

        #endregion
    }
}