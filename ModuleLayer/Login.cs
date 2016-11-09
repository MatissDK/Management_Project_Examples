namespace ModuleLayer
{
    public class Login
    {
        #region Static Fields

        public static int Id;

        public static string Name;

        public static string Surname;

        #endregion

        #region Constructors and Destructors

        public Login(int id, string name, string surname)
        {
            Id = id;
            Name = name;
            Surname = surname;
        }

        #endregion
    }
}