namespace NoteApplication.Models
{
    public class Note
    {
       public Guid NoteId { get; set; }
       public string? Title { get; set; }
       public string? Content { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime UpdatedAt { get; set; }
       public Guid UserId { get; set; }
       public User? User { get; set; }
    }
}
