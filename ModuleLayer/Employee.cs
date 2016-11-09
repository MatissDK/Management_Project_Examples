#region

#endregion

namespace ModuleLayer
{
    using System.Collections.Generic;

    public class Employee
    {
        #region Constructors and Destructors

        public Employee(int id, string name, string surName, string telNumber, string email, string address, List<Specialization> specializations, string zipCode)
        {
            this.Id = id;
            this.Name = name;
            this.SurName = surName;
            this.TelNumber = telNumber;
            this.Email = email;
            this.Address = address;
            this.ZipCode = zipCode;
            this.Specializations = specializations;
        }

        #endregion

        #region Public Properties

        public string Address { get; private set; }

        public string Email { get; private set; }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public List<Specialization> Specializations { get; private set; }

        public string SurName { get; private set; }

        public string TelNumber { get; private set; }

        public string ZipCode { get; private set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return this.Name + " " + this.SurName;
        }

        #endregion
    }
}