using BankSystem.Service.Services.AccountService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccountAsync([FromBody] int userId)
        {
            try
            {
                var account = await _accountService.CreateAccountAsync(userId);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("balance/{accountId}")]
        public async Task<IActionResult> GetAccountBalanceAsync(int accountId)
        {
            try
            {
                var balance = await _accountService.GetAccountBalanceAsync(accountId);
                return Ok(balance);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{accountId}")]
        public async Task<IActionResult> GetAccountDetailsByIdAsync(int accountId)
        {
            try
            {
                var account = await _accountService.GetAccountDetailsByIdAsync(accountId);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAccountDetailsByUserIdAsync(int userId)
        {
            try
            {
                var account = await _accountService.GetAccountDetailsByUserIdAsync(userId);
                return Ok(account);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("update/{accountId}")]
        public async Task<IActionResult> UpdateAccountDetailsAsync(int accountId, decimal newBalance, string accountStatus)
        {
            try
            {
                var result = await _accountService.UpdateAccountDetailsAsync(accountId, newBalance, accountStatus);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("status/{accountId}")]
        public async Task<IActionResult> UpdateAccountStatusAsync(int accountId, [FromBody] string status)
        {
            try
            {
                var result = await _accountService.UpdateAccountStatusAsync(accountId, status);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("tax/monthly/{accountId}")]
        public async Task<IActionResult> ApplyMonthlyTaxAsync(int accountId)
        {
            try
            {
                var newBalance = await _accountService.ApplyMonthlyTaxAsync(accountId);
                return newBalance.HasValue ? Ok(newBalance) : NotFound("Account not found or no tax applied.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("tax/annual/{accountId}")]
        public async Task<IActionResult> ApplyAnnualTaxAsync(int accountId)
        {
            try
            {
                var newBalance = await _accountService.ApplyAnnualTaxAsync(accountId);
                return newBalance.HasValue ? Ok(newBalance) : NotFound("Account not found or no tax applied.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("interest/monthly/{accountId}")]
        public async Task<IActionResult> ApplyMonthlyInterestAsync(int accountId)
        {
            try
            {
                var newBalance = await _accountService.ApplyMonthlyInterestAsync(accountId);
                return newBalance.HasValue ? Ok(newBalance) : NotFound("Account not found or no interest applied.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("interest/annual/{accountId}")]
        public async Task<IActionResult> ApplyAnnualInterestAsync(int accountId)
        {
            try
            {
                var newBalance = await _accountService.ApplyAnnualInterestAsync(accountId);
                return newBalance.HasValue ? Ok(newBalance) : NotFound("Account not found or no interest applied.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
