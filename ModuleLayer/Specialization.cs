namespace ModuleLayer
{
    public class Specialization
    {
        #region Constructors and Destructors

        public Specialization(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        #endregion

        #region Public Properties

        public int Id { get; private set; }

        public string Name { get; private set; }

        #endregion
    }
}