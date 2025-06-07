using System.ComponentModel.DataAnnotations;

namespace BankSystem.Service.Services.TransactionService.DTOs
{
    public class CustomerServiceConfirmDepositDto
    {
        [Required]
        public int TransactionId { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "OTP must be between 4 and 6 characters.")]
        public string OTP { get; set; }
    }
}
