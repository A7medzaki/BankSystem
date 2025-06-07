using System.ComponentModel.DataAnnotations;

namespace BankSystem.Service.Services.TransactionService.DTOs
{
    public class ConfirmTransferDto
    {
        [Required]
        public int SenderAccountId { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Receiver account number must be between 5 and 20 characters.")]
        public string ReceiverAccountNumber { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "OTP must be between 4 and 6 characters.")]
        public string OTP { get; set; }
    }
}
