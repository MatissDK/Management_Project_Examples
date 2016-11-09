#region

#endregion

namespace ModuleLayer
{
    public class Type
    {
        #region Constructors and Destructors

        public Type(int id, string typeName)
        {
            this.Id = id;
            this.TypeName = typeName;
        }

        #endregion

        #region Public Properties

        public int Id { get; private set; }

        public string TypeName { get; private set; }

        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return this.TypeName;
        }

        #endregion
    }
}