#region

#endregion

namespace ModuleLayer
{
    using System;

    public class Stock
    {
        #region Constructors and Destructors

        public Stock(DateTime date, Product product, int quantity, char actionType, Employee employee)
        {
            this.Date = date;
            this.Product = product;
            this.Quantity = quantity;
            this.ActionType = actionType;
            this.Employee = employee;
        }

        #endregion

        #region Public Properties

        public char ActionType { get; private set; }

        public Brand Brand
        {
            get
            {
                return this.Product.Brand;
            }
        }

        public DateTime Date { get; private set; }

        public Employee Employee { get; private set; }

        public Product Product { get; private set; }

        public int Quantity { get; private set; }

        public Type Type
        {
            get
            {
                return this.Product.Type;
            }
        }

        public int Volume
        {
            get
            {
                return this.Product.Volume;
            }
        }

        #endregion
    }
}