namespace BankSystem.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string HashedPassword { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime? UserCreatedAt { get; set; }
        public string? OTP { get; set; }
        public DateTime? OTPGeneratedAt { get; set; }
        public Account Account { get; set; }
    }
}
