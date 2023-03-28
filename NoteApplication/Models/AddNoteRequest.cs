namespace NoteApplication.Models
{
    public class AddNoteRequest
    {
        public string? Title { get; set; }
        public string? Message { get; set; }  
        public int MessageType { get; set; }
    }
}
