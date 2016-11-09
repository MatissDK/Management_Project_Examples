namespace GUI
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    using ControllerLayer;

    /// <summary>
    ///     Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region Static Fields

        public static Window MainWindow;

        #endregion

        #region Fields

        private readonly Label Label;

        private readonly LoginController LoginController;

        private Boolean LoginSuccessful;

        private TabControl Tab;

        #endregion

        #region Constructors and Destructors

        public LoginWindow(Window window, Label label, TabControl tab)
        {
            this.InitializeComponent();
            this.LoginController = new LoginController();
            MainWindow = window;
            this.Label = label;
            this.Tab = tab;
        }

        #endregion

        #region Methods

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string passwordBox = this.passwordTextBox.ToString();
            if (passwordBox != null && (this.loginTextBox.Text.Trim().Length != 0 && passwordBox.Trim().Length != 0))
            {
                string name = this.loginTextBox.Text;
                string password = this.passwordTextBox.Password;

                if (this.LoginController.VerifyPassword(name, password))
                {
                    this.Label.Content = this.LoginController.GetName();
                    this.LoginSuccessful = true;
                    MainWindow.Visibility = Visibility.Visible;
                    this.Close();
                }
                else
                    MessageBox.Show("Wrong username or password");
            }
            else
                MessageBox.Show("Make sure all fields have values");
        }

        private void LoginWindow_OnClosing(object sender, CancelEventArgs e)
        {
            if (!this.LoginSuccessful)
                Application.Current.Shutdown(0);
        }

        #endregion
    }
}