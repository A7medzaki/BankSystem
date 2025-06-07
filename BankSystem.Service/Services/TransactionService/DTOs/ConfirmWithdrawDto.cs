using System.ComponentModel.DataAnnotations;

namespace BankSystem.Service.Services.TransactionService.DTOs
{
    public class ConfirmWithdrawDto
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "OTP must be between 4 and 6 characters.")]
        public string OTP { get; set; }
    }
}
