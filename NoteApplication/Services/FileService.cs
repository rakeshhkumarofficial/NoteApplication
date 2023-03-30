
using Microsoft.EntityFrameworkCore;
using NoteApplication.Models;

namespace NoteApplication.Services
{
    public class FileService : IFileService
    {
        Response response = new Response();
        public Response FileUpload(FileUploadRequest upload, string email)
        {
            var fileName = Path.GetFileNameWithoutExtension(upload.File.FileName);
            var fileExt = Path.GetExtension(upload.File.FileName);
            var uniqueFileName = $"{fileName}_{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}{fileExt}";
            string FilePath = "wwwroot" + "//" + "Images" + "//" + uniqueFileName;

            string path = Path.Combine(Directory.GetCurrentDirectory(), FilePath);

            var filestream = System.IO.File.Create(path);
            upload.File.CopyTo(filestream);
            filestream.Close();

            response.Data = FilePath;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "File Uploaded Successfully..";
            return response;
        }
    }
}
