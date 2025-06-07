using BankSystem.Service.Services.TransactionService;
using BankSystem.Service.Services.TransactionService.DTOs;
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



        [HttpPost("withdraw/initiate")]
        public async Task<IActionResult> InitiateWithdrawAsync([FromBody] WithdrawRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _transactionService.InitiateWithdrawAsync(dto.AccountId, dto.Amount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw/confirm")]
        public async Task<IActionResult> ConfirmWithdrawAsync([FromBody] ConfirmWithdrawDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _transactionService.ConfirmWithdrawAsync(dto.AccountId, dto.Amount, dto.OTP);
                return result.success ? Ok(result.message) : BadRequest(result.message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("deposit/initiate")]
        public async Task<IActionResult> InitiateDepositAsync([FromBody] DepositRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _transactionService.InitiateDepositAsync(dto.AccountId, dto.Amount);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deposit/confirm")]
        public async Task<IActionResult> ConfirmDepositAsync([FromBody] ConfirmDepositDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _transactionService.ConfirmDepositAsync(dto.AccountId, dto.TransactionId, dto.OTP);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deposit/customer-service-confirm")]
        public async Task<IActionResult> CustomerServiceConfirmDepositAsync([FromBody] CustomerServiceConfirmDepositDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _transactionService.CustomerServiceConfirmDepositAsync(dto.TransactionId, dto.OTP);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost("initiate-transfer")]
        public async Task<IActionResult> InitiateTransfer([FromBody] InitiateTransferDto dto)
        {
            var result = await _transactionService.InitiateTransferFundsAsync(dto.SenderAccountId, dto.ReceiverAccountNumber, dto.Amount);
            if (result.StartsWith("Failed") || result.Contains("not found") || result.Contains("Insufficient") || result.Contains("must be"))
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("confirm-transfer")]
        public async Task<IActionResult> ConfirmTransfer([FromBody] ConfirmTransferDto dto)
        {
            var result = await _transactionService.ConfirmTransferFundsAsync(dto.SenderAccountId, dto.ReceiverAccountNumber, dto.Amount, dto.OTP);
            if (result.StartsWith("Failed") || result.Contains("not found") || result.Contains("Invalid") || result.Contains("Insufficient") || result.Contains("must be"))
                return BadRequest(result);

            return Ok(result);
        }
    }
}
