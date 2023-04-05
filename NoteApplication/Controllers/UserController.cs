using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteApplication.Models;
using NoteApplication.Services;
using System.Security.Claims;

namespace NoteApplication.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        public UserController(ILogger<UserController> logger , IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Register(RegisterRequest user)
        {
            _logger.LogInformation("\nExecuting method {MethodName}\n", nameof(Register));
            var res = _userService.Register(user);
            return Ok(res);
        }

        [HttpPost]
        public IActionResult Login(LoginRequest login)
        {
            _logger.LogInformation("\nExecuting method {MethodName}\n", nameof(Login));
            var res = _userService.Login(login);
            return Ok(res);
        }

        [HttpPut, Authorize(Roles = "Login")]
        public IActionResult UpdateProfile(UpdateProfileRequest update)
        {
            _logger.LogInformation("\nExecuting method {MethodName}\n", nameof(UpdateProfile));
            var user = HttpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var res = _userService.UpdateProfile(update, email);
            return Ok(res);
        }

        [HttpPut, Authorize(Roles = "Login")]
        public IActionResult ImageUpload([FromForm] FileUploadRequest upload)
        {
            _logger.LogInformation("\nExecuting method {MethodName}\n", nameof(ImageUpload));
            var user = HttpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var res = _userService.FileUpload(upload, email);
            return Ok(res);
        }

        [HttpPut, Authorize(Roles = "Login")]
        public IActionResult ChangePassword(ChangePasswordRequest changePasswordRequest)
        {
            _logger.LogInformation("\nExecuting method {MethodName}\n", nameof(ChangePassword));
            var user = HttpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var res = _userService.ChangePassword(changePasswordRequest, email);
            return Ok(res);
        }

        [HttpGet, Authorize(Roles = "Login")]
        public IActionResult GetUser()
        {
            _logger.LogInformation("\nExecuting method {MethodName}\n", nameof(GetUser));
            var user = HttpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var res = _userService.GetUser(email);
            return Ok(res);
        }
    }
}
