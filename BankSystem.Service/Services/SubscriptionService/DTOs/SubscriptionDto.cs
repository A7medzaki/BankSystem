using System.ComponentModel.DataAnnotations;

namespace BankSystem.Service.Services.SubscriptionService.DTOs
{
    public class SubscriptionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Subscription name is required.")]
        [StringLength(100, ErrorMessage = "Subscription name cannot exceed 100 characters.")]
        public string SubscriptionName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a non-negative value.")]
        public decimal Amount { get; set; }

        [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
        public decimal Discount { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? RenewalDate { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Account ID is required.")]
        public int AccountId { get; set; }

        public int? PartnerId { get; set; }
    }
}
