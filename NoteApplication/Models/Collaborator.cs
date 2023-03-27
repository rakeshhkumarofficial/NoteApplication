namespace NoteApplication.Models
{
    public class Collaborator
    {
        public Guid Id { get; set; }
        public string? SenderEmail { get; set; }
        public string? ReciverEmail { get; set; }
        public Note? Note { get; set; }
        public DateTime Time { get; set; }  
    }
}
