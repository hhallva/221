using System.Collections.ObjectModel;

namespace LabWork7
{
    public class EventService
    {
        private readonly ObservableCollection<EventItem> _events = new();

        public ObservableCollection<EventItem> Events => _events;

        public void AddEvent(DateTime dateTime, string description)
        {
            _events.Add(new EventItem { DateTime = dateTime, Description = description });
        }

        public void RemoveEvent(EventItem item)
        {
            _events.Remove(item);
        }

        public void UpdateEvent(EventItem item, DateTime newDateTime, string newDescription)
        {
            item.DateTime = newDateTime;
            item.Description = newDescription;
        }

        public List<EventItem> GetAllEvents() => new(_events);
    }
}
