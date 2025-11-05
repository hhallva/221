using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GameStore
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string message, string caption, MessageBoxButton button, MessageBoxImage icon)
        {
            Message = message;
            Caption = caption;
            Button = button;
            Icon = icon;
        }

        public string Message { get; set; }
        public string Caption { get; set; }
        public MessageBoxButton Button { get; set; }
        public MessageBoxImage Icon { get; set; }
    }
}
