using System.Windows;


namespace LabWork7
{
    public class Automation(EventService eventService)
    {
        private readonly EventService _eventService = eventService;

        public void Message(string message)
        {
            MessageBox.Show(message, "Automation Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // CRUD-методы для событий

        public void AddEvent(DateTime dateTime, string description)
        {
            _eventService.AddEvent(dateTime, description);
        }

        public void RemoveEvent(int index)
        {
            var events = _eventService.GetAllEvents();
            if (index >= 0 && index < events.Count)
            {
                _eventService.RemoveEvent(events[index]);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс события вне диапазона.");
            }
        }

        public void UpdateEvent(int index, DateTime newDateTime, string newDescription)
        {
            var events = _eventService.GetAllEvents();
            if (index >= 0 && index < events.Count)
            {
                _eventService.UpdateEvent(events[index], newDateTime, newDescription);
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Индекс события вне диапазона.");
            }
        }

        public List<EventItem> GetEvents()
        {
            return _eventService.GetAllEvents();
        }
    }
}