using System.Security.Cryptography;
using System.Text;

namespace BankSystem.Service.Helper
{
    public class OTPService
    {
        public string GenerateOTP(int length = 6)
        {
            var otp = new StringBuilder();
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] randomNumber = new byte[1];

                while (otp.Length < length)
                {
                    rng.GetBytes(randomNumber);
                    int digit = randomNumber[0] % 10;
                    otp.Append(digit);
                }
            }

            return otp.ToString();
        }

        public bool ValidateOTP(string inputOtp, string actualOtp, DateTime generatedAt, int expirationMinutes = 5)
        {
            if (string.IsNullOrWhiteSpace(inputOtp) || string.IsNullOrWhiteSpace(actualOtp))
                return false;

            if (DateTime.Now > generatedAt.AddMinutes(expirationMinutes))
                return false;

            return inputOtp == actualOtp;
        }
    }
}
