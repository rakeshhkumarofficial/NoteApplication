namespace NoteApplication.Models
{
    public class ReminderResponse
    {
        public DateTime ReminderTime { get; set; } 
        public string NoteId { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Image { get ; set; }

    }
}
