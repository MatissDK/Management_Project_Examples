namespace ControllerLayer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    using ModuleLayer;

    public class LoginController
    {
        #region Static Fields

        private static MD5 Md5Hash;

        #endregion

        #region Constructors and Destructors

        public LoginController()
        {
            Md5Hash = MD5.Create();
        }

        #endregion

        #region Public Methods and Operators

        public string GetMd5Hash(string input)
        {
            byte[] data = Md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();

            foreach (byte t in data)
                sBuilder.Append(t.ToString("x2"));
            return sBuilder.ToString();
        }

        public string GetName()
        {
            return Login.Name + " " + Login.Surname;
        }

        public Boolean VerifyPassword(string name, string password)
        {
            var paramsList = new List<SqlParameter> { new SqlParameter("@Name", name), new SqlParameter("@Hash", GetMd5Hash(password)) };

            const string ProcedureName = "GetHash";
            List<object> ActiveUser = new DBController().GetFromDatabase(ProcedureName, "Login", "EJL15_DB", paramsList).ToList();
            return ActiveUser.Count > 0;
        }

        #endregion
    }
}