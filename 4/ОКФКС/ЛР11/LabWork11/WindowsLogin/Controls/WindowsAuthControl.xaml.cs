using System.Security.Principal;
using System.Windows;
using System.Windows.Controls;

namespace WindowsLogin
{
    /// <summary>
    /// Логика взаимодействия для WindowsAuthControl.xaml
    /// </summary>
    public partial class WindowsAuthControl : UserControl
    {
        public WindowsAuthControl()
        {
            InitializeComponent();
            DisplayCurrentUser();
        }

        private void DisplayCurrentUser()
        {
            var identity = WindowsIdentity.GetCurrent();
            CurrentUserText.Text = identity.Name;
        }

        private void AuthenticateButton_Click(object sender, RoutedEventArgs e)
        {
            var identity = WindowsIdentity.GetCurrent();

            if (identity.IsAuthenticated)
            {
                ResultText.Text = $"Успешная аутентификация! Пользователь: {identity.Name}";
                ResultText.Foreground = System.Windows.Media.Brushes.Green;
            }
            else
            {
                ResultText.Text = "Ошибка аутентификации Windows";
                ResultText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}
