using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectLaborBackend.Dtos.UserDTOs;
using ProjectLaborBackend.Entities;
using ProjectLaborBackend.Services;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProjectLaborBackend.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IUserService userService;

        public UserController(AppDbContext _context, IUserService _userService)
        {
            context = _context;
            userService = _userService;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserGetDTO>>> GetUsers()
        {
            return await userService.GetUsersAsync();
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserGetDTO>> GetUser(int id)
        {
            try
            {
                var user = await userService.GetUserByIdAsync(id);
                return Ok(user);
            }
            catch (KeyNotFoundException e) { return NotFound(e.Message); }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userDto)
        {
            try
            {
                var result = await userService.RegisterAsync(userDto);
                return Ok(result);
            }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userDto)
        {
            try
            {
                return Ok();
            }
            catch (UnauthorizedAccessException e) { return Unauthorized(e.Message); }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        [HttpPut("update-profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int userId, [FromBody] UserPutDTO userDto)
        {
            try
            {
                var result = await userService.UpdateProfileAsync(userId, userDto);
                return Ok(result);
            }
            catch (KeyNotFoundException e) { return NotFound(e.Message); }
            catch (Exception e) { return BadRequest(e.Message); }
        }

        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await userService.DeleteUser(id);
                return Ok();
            }
            catch (KeyNotFoundException e) { return NotFound(e.Message); }
            catch (Exception e) { return BadRequest(e.Message); }
        }
    }
}
