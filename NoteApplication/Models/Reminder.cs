using System.ComponentModel.DataAnnotations;

namespace NoteApplication.Models
{
    public class Reminder
    {
        [Key]
        public Guid RemId { get; set; }
        public Guid NoteId { get; set; }
        public DateTime RemindAt { get; set; }
        public string? Email { get; set; }
    }
}
