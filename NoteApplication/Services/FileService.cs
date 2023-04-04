
using Microsoft.EntityFrameworkCore;
using NoteApplication.Models;

namespace NoteApplication.Services
{
    public class FileService : IFileService
    {
        Response response = new Response();
        public Response FileUpload(FileUploadRequest upload, string email)
        {
            string fileName = Path.GetFileNameWithoutExtension(upload.File.FileName);
            string fileExt = Path.GetExtension(upload.File.FileName);
            string uniqueFileName = $"{fileName}_{DateTime.UtcNow.ToString("yyyyMMddHHmmssfff")}{fileExt}";
            string folderName = "wwwroot//Images//";
            string path = Path.Combine(Directory.GetCurrentDirectory(), folderName);       
            var fullpath = path + "//" + uniqueFileName;
            var filestream = File.Create(fullpath);
            upload.File.CopyTo(filestream);
            filestream.Close();

            response.Data = folderName + uniqueFileName;
            response.StatusCode = 200;
            response.IsSuccess = true;
            response.Message = "File Uploaded Successfully..";
            return response;
        }
    }
}
