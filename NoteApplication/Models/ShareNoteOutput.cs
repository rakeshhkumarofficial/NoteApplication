namespace NoteApplication.Models
{
    public class ShareNoteOutput
    {
        public Guid NoteId { get; set; }
        public string? Title { get; set; }
        public string? Text { get; set; }
        public string? Images { get; set; }
        public int? MessageType { get; set; }
        public string? CreatorEmail { get; set; }
        public bool IsVisible { get; set; }
    }
}
