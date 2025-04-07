using BankSystem.Service.Services.TransactionService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BankSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("history/{accountId}")]
        public async Task<IActionResult> GetTransactionHistoryAsync(int accountId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var transactions = await _transactionService.GetTransactionHistoryAsync(accountId, startDate, endDate);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetTransactionDetailsAsync(int transactionId)
        {
            try
            {
                var transaction = await _transactionService.GetTransactionDetailsAsync(transactionId);
                if (transaction == null) return NotFound("Transaction not found.");
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("cancel/{transactionId}")]
        public async Task<IActionResult> CancelTransactionAsync(int transactionId)
        {
            try
            {
                var result = await _transactionService.CancelTransactionAsync(transactionId);
                if (!result) return NotFound("Transaction cannot be canceled.");
                return Ok("Transaction canceled successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("status/{transactionId}")]
        public async Task<IActionResult> GetTransactionStatusAsync(int transactionId)
        {
            try
            {
                var status = await _transactionService.GetTransactionStatusAsync(transactionId);
                return Ok(status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw/{accountId}")]
        public async Task<IActionResult> InitiateWithdrawAsync(int accountId, [FromQuery] decimal amount)
        {
            try
            {
                var result = await _transactionService.InitiateWithdrawAsync(accountId, amount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw/confirm")]
        public async Task<IActionResult> ConfirmWithdrawAsync([FromQuery] int accountId, [FromQuery] decimal amount, [FromQuery] string otp)
        {
            try
            {
                var result = await _transactionService.ConfirmWithdrawAsync(accountId, amount, otp);
                return result.success ? Ok(result.message) : BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deposit/{accountId}")]
        public async Task<IActionResult> InitiateDepositAsync(int accountId, [FromQuery] decimal amount)
        {
            try
            {
                var result = await _transactionService.InitiateDepositAsync(accountId, amount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deposit/confirm")]
        public async Task<IActionResult> ConfirmDepositAsync([FromQuery] int accountId, [FromQuery] int transactionId, [FromQuery] string otp)
        {
            try
            {
                var result = await _transactionService.ConfirmDepositAsync(accountId, transactionId, otp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deposit/customer-service-confirm")]
        public async Task<IActionResult> CustomerServiceConfirmDepositAsync([FromQuery] int transactionId, [FromQuery] string otp)
        {
            try
            {
                var result = await _transactionService.CustomerServiceConfirmDepositAsync(transactionId, otp);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
