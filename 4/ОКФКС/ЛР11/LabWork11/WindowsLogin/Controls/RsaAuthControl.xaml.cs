using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using WindowsLogin.Services;

namespace WindowsLogin
{
    public partial class RsaAuthControl : UserControl
    {
        private DatabaseService _databaseService;

        public RsaAuthControl()
        {
            InitializeComponent();
            _databaseService = new DatabaseService();
        }

        private void RsaRegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var username = RsaRegUsername.Text;
            var role = (RsaRegRole.SelectedItem as ComboBoxItem).Content.ToString();

            if (string.IsNullOrEmpty(username))
            {
                RsaAuthResult.Text = "Введите имя пользователя";
                RsaAuthResult.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            try
            {
                // Генерация ключей RSA
                using var rsa = RSA.Create();
                byte[] publicKey = rsa.ExportRSAPublicKey();
                byte[] privateKey = rsa.ExportRSAPrivateKey();

                // Генерация тестового токена
                byte[] testToken = RandomNumberGenerator.GetBytes(32);

                // Шифрование тестового токена публичным ключом
                rsa.ImportRSAPublicKey(publicKey, out _);
                byte[] encryptedToken = rsa.Encrypt(testToken, RSAEncryptionPadding.Pkcs1);

                // Сохранение в базу
                if (_databaseService.RegisterUserWithRsa(username, publicKey, encryptedToken, role))
                {
                    // Предложение сохранить приватный ключ
                    var saveDialog = new SaveFileDialog
                    {
                        Filter = "Private Key files (*.key)|*.key",
                        FileName = $"{username}_private.key"
                    };

                    if (saveDialog.ShowDialog() == true)
                    {
                        File.WriteAllBytes(saveDialog.FileName, privateKey);
                        PrivateKeyInfo.Text = $"Приватный ключ сохранен: {saveDialog.FileName}\nСохраните его в безопасном месте!";

                        RsaAuthResult.Text = $"Пользователь {username} успешно зарегистрирован с ролью {role}";
                        RsaAuthResult.Foreground = System.Windows.Media.Brushes.Green;

                        RsaRegUsername.Text = "";
                    }
                }
                else
                {
                    RsaAuthResult.Text = "Ошибка регистрации. Возможно, пользователь уже существует.";
                    RsaAuthResult.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                RsaAuthResult.Text = $"Ошибка: {ex.Message}";
                RsaAuthResult.Foreground = System.Windows.Media.Brushes.Red;
            }
        }

        private void BrowsePrivateKey_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                Filter = "Private Key files (*.key)|*.key"
            };

            if (openDialog.ShowDialog() == true)
            {
                PrivateKeyPath.Text = openDialog.FileName;
            }
        }

        private void RsaLoginButton_Click(object sender, RoutedEventArgs e)
        {
            var username = RsaLoginUsername.Text;
            var privateKeyPath = PrivateKeyPath.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(privateKeyPath))
            {
                RsaAuthResult.Text = "Заполните все поля";
                RsaAuthResult.Foreground = System.Windows.Media.Brushes.Red;
                return;
            }

            try
            {
                var user = _databaseService.GetUserForRsaAuth(username);
                if (user == null)
                {
                    RsaAuthResult.Text = "Пользователь не найден или не зарегистрирован с RSA ключами";
                    RsaAuthResult.Foreground = System.Windows.Media.Brushes.Red;
                    return;
                }

                // Загрузка приватного ключа
                byte[] privateKeyBytes = File.ReadAllBytes(privateKeyPath);

                using var rsa = RSA.Create();
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

                // Попытка расшифровки токена
                try
                {
                    byte[] decryptedToken = rsa.Decrypt(user.EncryptedToken, RSAEncryptionPadding.Pkcs1);

                    // Если расшифровка прошла успешно (без исключения)
                    RsaAuthResult.Text = $"Успешная авторизация! Пользователь: {user.Username}, Роль: {user.Role}";
                    RsaAuthResult.Foreground = System.Windows.Media.Brushes.Green;

                    RsaLoginUsername.Text = "";
                    PrivateKeyPath.Text = "";
                }
                catch
                {
                    RsaAuthResult.Text = "Ошибка авторизации. Неверный приватный ключ.";
                    RsaAuthResult.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            catch (Exception ex)
            {
                RsaAuthResult.Text = $"Ошибка: {ex.Message}";
                RsaAuthResult.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}

