using BankSystem.Data;
using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Service.Services;
using BankSystem.Service.Services.CheckService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace BankSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChequeController : ControllerBase
    {
        private readonly IChequeService _chequeService;
        private readonly BankingContext _dbContext;

        public ChequeController(IChequeService chequeService, BankingContext dbContext)
        {
            _chequeService = chequeService;
            _dbContext = dbContext;
        }
        // POST api/cheque/generate
        [HttpPost("generate")]
        public async Task<IActionResult> GenerateCheque([FromQuery] string fromAccountName, [FromQuery] string toName, [FromQuery] decimal amount, [FromQuery] string chequeNumber)
        {
            if (string.IsNullOrEmpty(fromAccountName) || string.IsNullOrEmpty(chequeNumber))
                return BadRequest("All fields are required.");

            if (amount <= 0)
                return BadRequest("Amount must be greater than zero.");

            var user = await _dbContext.Users
                                       .Include(u => u.Account)
                                       .FirstOrDefaultAsync(u => u.UserName == fromAccountName);
            if (user == null)
                return NotFound("Sender account not found.");

            var fromAccount = user.Account;

            if (fromAccount.Balance < amount)
                return BadRequest("Insufficient balance.");

            try
            {
                var chequePdf = await _chequeService.GenerateChequePdfAsync(fromAccountName, toName, amount, chequeNumber);
                return File(chequePdf, "application/pdf", $"{chequeNumber}_Cheque.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        
    }
}
