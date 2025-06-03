using System.ComponentModel.DataAnnotations;

namespace BankSystem.Data.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "UserName Is Required")]
        public string? UserName { get; set; }


        [Required(ErrorMessage = "Email Is Required")]
        [EmailAddress(ErrorMessage = "Invalid Format For Email")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "Password Is Required")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$", ErrorMessage = "Password must be at least 6 characters long, contain at least one lowercase letter, one uppercase letter, one digit, and one non-alphanumeric character.")]
        [DataType(DataType.Password)]
        public string HashedPassword { get; set; } = string.Empty;

        public string? Role { get; set; } = "User";


        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone Number must be exactly 11 digits.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; } = string.Empty;


        public DateTime? UserCreatedAt { get; set; }

        public string? FacebookId { get; set; }

        public string? Gmail { get; set; }





        public string? OTP { get; set; }
        public DateTime? OTPGeneratedAt { get; set; }
        public Account Account { get; set; }

        public List<Complain> Complains { get; set; } = new();

    }
}
