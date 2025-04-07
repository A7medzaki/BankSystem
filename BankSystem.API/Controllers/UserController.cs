using BankSystem.Service.Services.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        //[HttpPost("register")]
        ///public async Task<IActionResult> RegisterUserAsync([FromBody] RegisterRequest request)
        //{
        //    try
        //    {
        //        var result = await _userService.RegisterUserAsync(request.Email, request.Password);
        //        if (result == "User already exists.")
        //        {
        //            return Conflict(result);  // 409 Conflict
        //        }

        //        return Ok(result);  // 200 OK
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);  // 400 Bad Request
        //    }
        //}

        // Get User by ID

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);  // 404 Not Found
            }
        }

        [HttpGet("with-account/{userId}")]
        public async Task<IActionResult> GetUserWithAccountAsync(int userId)
        {
            try
            {
                var user = await _userService.GetUserWithAccountAsync(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);  // 404 Not Found
            }
        }

        //[HttpPut("{userId}")]
        ///public async Task<IActionResult> UpdateUserAsync(int userId, [FromBody] UpdateUserRequest request)
        //{
        //    try
        //    {
        //        var result = await _userService.UpdateUserAsync(userId, request.Email, request.Name);
        //        return Ok(result);
        //    }
        //    catch (Exception ex)
        //    {
        //        return NotFound(ex.Message);  // 404 Not Found
        //    }
        //}

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUserAsync(int userId)
        {
            try
            {
                var result = await _userService.DeleteUserAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);  // 404 Not Found
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);  // 400 Bad Request
            }
        }
    }
}