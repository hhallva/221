using System.ComponentModel;
using System.Windows;

namespace GameStore.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler<MessageEventArgs> SendMessage;

        protected void OnSendMessage(string message, string caption = "", MessageBoxButton button = MessageBoxButton.OK, MessageBoxImage icon = MessageBoxImage.None)
        {
            SendMessage?.Invoke(this, new MessageEventArgs(message, caption, button, icon));
        }
    }
}