namespace NoteApplication.Models
{
    public class AddNoteRequest
    {
        public string? Title { get; set; }
        public string? Content { get; set; }  
        public int ContentType { get; set; }
    }
}
