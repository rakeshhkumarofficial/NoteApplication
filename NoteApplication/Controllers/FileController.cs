using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NoteApplication.Models;
using NoteApplication.Services;
using System.Data;
using System.Security.Claims;

namespace NoteApplication.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;
        public FileController(ILogger<FileController> logger, IFileService fileService)
        {
            _logger = logger;
            _fileService = fileService;
        }
        [HttpPost, Authorize(Roles = "Login")]
        public IActionResult FileUpload([FromForm] FileUploadRequest upload)
        {
            _logger.LogInformation("\nExecuting method {MethodName}\n", nameof(FileUpload));
            var user = HttpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var res = _fileService.FileUpload(upload, email);
            return Ok(res);
        }
    }
}
