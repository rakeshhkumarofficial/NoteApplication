namespace NoteApplication.Models
{
    public class Note
    {
       public Guid NoteId { get; set; }
       public string? Title { get; set; }
       public string? Text { get; set; }
       public string? Images { get; set; }
       public string? CreatorEmail { get; set; }
       public DateTime CreatedAt { get; set; }
       public DateTime UpdatedAt { get; set; }
    }
}
