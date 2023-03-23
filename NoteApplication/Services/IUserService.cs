using NoteApplication.Models;

namespace NoteApplication.Services
{
    public interface IUserService
    {
        public Response Register(RegisterRequest user);
        public Response Login(LoginRequest login);

        public Response UpdateProfile(UpdateProfileRequest update, string email);
        public Response ChangePassword(ChangePasswordRequest changePasswordRequest, string email);
    }
}
