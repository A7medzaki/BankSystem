using BankSystem.Data.Entities;
using BankSystem.Service.Services.ComplainService;
using Microsoft.AspNetCore.Mvc;




namespace BankSystem.API.Controllers.STC
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplainsController : ControllerBase
    {
        private readonly IComplainService _service;

        public ComplainsController(IComplainService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComplain([FromBody] CreateComplainDto dto)
        {
            if (dto == null)
                return BadRequest("Complain is null.");

            var complain = new Complain
            {
                Describtion = dto.Describtion,
                Recipient = dto.Recipient,
                Solved = dto.Solved,
                Timestamp = dto.Timestamp,
                EndDate = dto.EndDate,
                UserId = dto.UserId
            };

            var created = await _service.CreateComplainAsync(complain);
            return CreatedAtAction(nameof(GetComplainById), new { id = created.Id }, created);
        }

        [HttpGet]
        public async Task<IActionResult> GetComplains() =>
            Ok(await _service.GetAllComplainsAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetComplainById(int id)
        {
            var complain = await _service.GetComplainByIdAsync(id);
            if (complain == null) return NotFound("Complain not found.");
            return Ok(complain);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateComplain(int id, [FromBody] Complain updatedComplain)
        {
            if (updatedComplain == null || id != updatedComplain.Id)
                return BadRequest("Invalid data.");

            var success = await _service.UpdateComplainAsync(id, updatedComplain);
            return success ? Ok(updatedComplain) : NotFound("Complain not found.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComplain(int id)
        {
            var success = await _service.DeleteComplainAsync(id);
            return success ? Ok(new { Message = "Deleted successfully." }) : NotFound("Complain not found.");
        }
    }
}
