using NoteApplication.Models;

namespace NoteApplication.Services
{
    public interface IFileService
    {
        public Response FileUpload(FileUploadRequest upload, string email);
    }
}
