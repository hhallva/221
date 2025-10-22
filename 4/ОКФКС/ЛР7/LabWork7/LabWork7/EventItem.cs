using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LabWork7
{
    public class EventItem : INotifyPropertyChanged
    {
        private DateTime _dateTime;
        private string _description = string.Empty;

        public DateTime DateTime
        {
            get => _dateTime;
            set
            {
                if (_dateTime != value)
                {
                    _dateTime = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged();
                }
            }
        }

        public override string ToString()
        {
            return $"{DateTime:dd.MM.yyyy HH:mm} — {Description}";
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
