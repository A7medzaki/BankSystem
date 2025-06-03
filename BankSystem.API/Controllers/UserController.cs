using BankSystem.Data.Entities;
using BankSystem.Service.Services;
using BankSystem.Service.Services.UserService.BankSystem.Service.Services.UserService;
using Microsoft.AspNetCore.Mvc;

namespace STC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var (exists, reason) = await _service.CheckDuplicateAsync(userDto.Email, userDto.Gmail, userDto.FacebookId);
            if (exists) return BadRequest(reason);

            var user = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                HashedPassword = userDto.HashedPassword ,
                PhoneNumber = userDto.PhoneNumber,
                Gmail = userDto.Gmail?? String.Empty,
                FacebookId = userDto.FacebookId?? String.Empty,
                UserCreatedAt = DateTime.UtcNow
            };

            await _service.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPost("Google")]
        public async Task<IActionResult> GoogleRegister([FromBody] UserRegisterDto googleDto)
        {
            var (exists, reason) = await _service.CheckDuplicateAsync(googleDto.Email, googleDto.Gmail);
            if (exists) return BadRequest(reason);

            var user = new User
            {
                UserName = googleDto.UserName,
                Email = googleDto.Email,
                Gmail = googleDto.Gmail,
                HashedPassword = "gmail registration",
                PhoneNumber = "gmail registration",
                UserCreatedAt = DateTime.UtcNow
            };

            await _service.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpPost("Facebook")]
        public async Task<IActionResult> FacebookRegister([FromBody] UserRegisterDto fbDto)
        {
            var (exists, reason) = await _service.CheckDuplicateAsync(fbDto.Email, null, fbDto.FacebookId);
            if (exists) return BadRequest(reason);

            var user = new User
            {
                UserName = fbDto.UserName,
                Email = fbDto.Email,
                FacebookId = fbDto.FacebookId,
                HashedPassword = "facebook registration",
                PhoneNumber = "facebook registration",
                UserCreatedAt = DateTime.UtcNow
            };

            await _service.CreateUserAsync(user);
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers() => Ok(await _service.GetAllUsersAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _service.GetUserByIdAsync(id);
            return user == null ? NotFound("User not found.") : Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id) return BadRequest("ID mismatch.");
            var updated = await _service.UpdateUserAsync(id, user);
            return updated ? NoContent() : NotFound("User not found.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var deleted = await _service.DeleteUserAsync(id);
            return deleted ? NoContent() : NotFound("User not found.");
        }
    }
}
