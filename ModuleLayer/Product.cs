namespace ModuleLayer
{
    public class Product
    {
        #region Constructors and Destructors

        public Product(int id, Brand brand, Type type, string name, int volume, int quantity)
        {
            this.Id = id;
            this.Brand = brand;
            this.Type = type;
            this.Name = name;
            this.Volume = volume;
            this.Quantity = quantity;
        }

        #endregion

        #region Public Properties

        public Brand Brand { get; private set; }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public int Quantity { get; set; }

        public Type Type { get; private set; }

        public int Volume { get; private set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return this.Name;
        }

        #endregion
    }
}