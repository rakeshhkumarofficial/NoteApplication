using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteApplication.Models;
using NoteApplication.Services;
using System.Security.Claims;

namespace NoteApplication.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {

        private readonly IPasswordService _passwordService;
        private readonly ILogger<PasswordController> _logger;
        public PasswordController(ILogger<PasswordController> logger , IPasswordService passwordService)
        {
            _logger = logger;
            _passwordService = passwordService;

        }

        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordRequest forget)
        {
            _logger.LogInformation("\nExecuting method {MethodName}\n", nameof(ForgetPassword));
            var res = _passwordService.ForgetPassword(forget);
            return Ok(res);
        }

        [HttpPost, Authorize(Roles = "Reset")]
        public IActionResult ResetPassword(ResetPasswordRequest reset)
        {
            _logger.LogInformation("\nExecuting method {MethodName}\n", nameof(ResetPassword));
            var user = HttpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var res = _passwordService.ResetPassword(reset, email);
            return Ok(res);
        }
    }
}
