namespace LabWork7
{
    public class EventItem
    {
        public DateTime DateTime { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"{DateTime:dd.MM.yyyy HH:mm} — {Description}";
        }
    }
}
