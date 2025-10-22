using System.IO;
using System.Windows;

namespace LabWork7
{
    public partial class MainWindow : Window
    {
        private readonly ScriptingHost _scriptingHost;
        private readonly CancellationTokenSource _cts = new();
        private readonly EventService _eventService = new();

        public MainWindow()
        {
            InitializeComponent();
            _scriptingHost = new ScriptingHost(_eventService);
            EventsDataGrid.ItemsSource = _eventService.Events;
            ScriptTextBox.Text = "// Добавить событие\r\nAddEvent(DateTime.Now.AddDays(1), \"Встреча с заказчиком\");\r\n\r\n// Вывести все события\r\nvar events = GetEvents();\r\nforeach (var ev in events)\r\n{\r\n    Message(ev.ToString());\r\n}\r\n\r\n// Обновить первое событие\r\nUpdateEvent(0, DateTime.Now.AddDays(2), \"Перенос встречи\");\r\n\r\n// Удалить последнее событие\r\nvar all = GetEvents();\r\nif (all.Count > 0)\r\n    RemoveEvent(all.Count - 1);";

            _ = Task.Run(() => _scriptingHost.Run(_cts.Token));
        }

        private async void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            string code = ScriptTextBox.Text?.Trim();

            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Введите код скрипта.", "Пустой ввод", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                await _scriptingHost.ExecuteAsync(code);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка:\n{ex.Message}", "Ошибка выполнения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadScriptFromFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new()
            {
                DefaultExt = ".txt",
                Filter = "Текстовые файлы (*.txt)|*.txt|Скрипты (*.csx)|*.csx|Все файлы (*.*)|*.*"
            };

            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                try
                {
                    string script = File.ReadAllText(dlg.FileName);
                    ScriptTextBox.Text = script;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось загрузить файл:\n{ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}