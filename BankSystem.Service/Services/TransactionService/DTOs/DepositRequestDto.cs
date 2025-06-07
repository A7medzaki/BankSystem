using System.ComponentModel.DataAnnotations;

namespace BankSystem.Service.Services.TransactionService.DTOs
{
    public class DepositRequestDto
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
    }
}
