using System.Windows;
using System.Windows.Controls;
using WindowsLogin.Services;

namespace WindowsLogin
{
    public partial class DatabaseAuthControl : UserControl
    {
        private DatabaseService _databaseService;

        public DatabaseAuthControl()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var username = RegUsername.Text;
            var password = (RegPassword as PasswordBox).Password;
            var role = (RegRole.SelectedItem as ComboBoxItem).Content.ToString();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                DbAuthResult.Text = "Заполните все поля";
                DbAuthResult.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            if (_databaseService.RegisterUserWithPassword(username, password, role))
            {
                DbAuthResult.Text = $"Пользователь {username} успешно зарегистрирован с ролью {role}";
                DbAuthResult.Foreground = System.Windows.Media.Brushes.Green;

                RegUsername.Text = "";
                (RegPassword as PasswordBox).Password = "";
            }
            else
            {
                DbAuthResult.Text = "Ошибка регистрации. Возможно, пользователь уже существует.";
                DbAuthResult.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = LoginUsername.Text;
            var password = (LoginPassword as PasswordBox).Password;

            var user = _databaseService.AuthenticateWithPassword(username, password);

            if (user != null)
            {
                DbAuthResult.Text = $"Успешный вход! Пользователь: {user.Username}, Роль: {user.Role}";
                DbAuthResult.Foreground = System.Windows.Media.Brushes.Green;

                LoginUsername.Text = "";
                (LoginPassword as PasswordBox).Password = "";
            }
            else
            {
                DbAuthResult.Text = "Ошибка аутентификации. Неверное имя пользователя или пароль.";
                DbAuthResult.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
