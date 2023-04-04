namespace NoteApplication.Models
{
    public class Note
    {
       public Guid NoteId { get; set; }
       public string? Title { get; set; }
       public string? Text { get; set; }
       public string? Images { get; set; }
       public int? MessageType { get; set; }
       public string? CreatorEmail { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime UpdatedAt { get; set; }
       public bool IsArchived { get; set; }
       public bool IsTrashed { get; set; }
       public int Pin { get; set; }
       public bool IsShared { get; set; } 
        public bool IsVisible { get; set; }
       public bool IsReminder { get; set; }
    }
}
