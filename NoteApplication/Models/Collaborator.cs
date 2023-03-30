using System.ComponentModel.DataAnnotations;

namespace NoteApplication.Models
{
    public class Collaborator
    {
        [Key]
        public Guid Id { get; set; }
        public string? SenderEmail { get; set; }
        public string? ReciverEmail { get; set; }
        public Guid NoteId { get; set; }
        public DateTime Time { get; set; }  
    }
}
