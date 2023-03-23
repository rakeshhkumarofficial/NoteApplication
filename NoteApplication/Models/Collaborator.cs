namespace NoteApplication.Models
{
    public class Collaborator
    {
        public Guid Id { get; set; }
        public Guid NoteId { get; set; }
        public Note? Note { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }

    }
}
