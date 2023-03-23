namespace NoteApplication.Models
{
    public class Reminder
    {
        public Guid RemId { get; set; }
        public int NoteId { get; set; }
        public Note? Note { get; set; }
        public DateTime RemindAt { get; set; }
    }
}
