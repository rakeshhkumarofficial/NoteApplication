using NoteApplication.Models;

namespace NoteApplication.Services
{
    public interface IPasswordService
    {
        public Response ForgetPassword(ForgetPasswordRequest forget);
        public Response ResetPassword(ResetPasswordRequest reset, string email);
    }
}
