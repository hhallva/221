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
            _scriptingHost = new ScriptingHost();
            EventsDataGrid.ItemsSource = _eventService.Events;

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

        protected override void OnClosed(EventArgs e)
        {
            _cts.Cancel();
            base.OnClosed(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LoadScriptFromFile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}