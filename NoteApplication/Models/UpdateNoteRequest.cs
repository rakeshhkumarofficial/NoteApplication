namespace NoteApplication.Models
{
    public class UpdateNoteRequest
    {
        public string? NoteId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public int MessageType { get; set; } = -1;
        public string? URL { get; set; } = null;
    }
}
