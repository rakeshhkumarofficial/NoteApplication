using System.ComponentModel.DataAnnotations;

namespace NoteApplication.Models
{
    public class ForgetPassword
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? ResetPasswordToken { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
