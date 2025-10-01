using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using ProjectLaborBackend.Dtos.UserDTOs;
using ProjectLaborBackend.Email;
using ProjectLaborBackend.Services;

namespace ProjectLaborBackend.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send-welcome")]
        public async Task<IActionResult> SendWelcomeEmail([FromBody] UserEmailDto emailDto)
        {
            try
            {
                string templatePath = $"{Directory.GetCurrentDirectory()}/Email/Templates/Welcome.cshtml";
                await _emailService.SendEmail(emailDto.Email, "Üdvözlünk a RakLapnál!", templatePath);
                return Ok("Welcome email sent successfully!");
            }
            catch (KeyNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("send-confirm")]
        public async Task<IActionResult> SendConfirmEmail([FromBody] UserEmailDto emailDto)
        {
            try
            {
                string templatePath = $"{Directory.GetCurrentDirectory()}/Email/Templates/Welcome.cshtml";
                await _emailService.SendEmail(emailDto.Email, "Erősítsd meg email címed", templatePath);
                return Ok("Email confirm sent successfully!");
            }
            catch (KeyNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("send-password-reset")]
        public async Task<IActionResult> SendPasswordResetEmail([FromBody] UserEmailDto emailDto)
        {
            try
            {
                string templatePath = $"{Directory.GetCurrentDirectory()}/Email/Templates/PasswordReset.cshtml";
                await _emailService.SendEmail(emailDto.Email, "Jelszó visszaállítás", templatePath);
                return Ok("Password reset email sent successfully!");
            }
            catch (KeyNotFoundException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
