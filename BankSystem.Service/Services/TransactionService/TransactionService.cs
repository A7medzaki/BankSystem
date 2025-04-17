using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Service.Helper;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Service.Services.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly BankingContext _context;
        private readonly OTPService _otpService;
        private readonly EmailService _emailService;

        public TransactionService(BankingContext context, OTPService otpService, EmailService emailService)
        {
            _context = context;
            _otpService = otpService;
            _emailService = emailService;
        }

        public async Task<List<Transaction>> GetTransactionHistoryAsync(int accountId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Transactions
                .Where(t => t.AccountID == accountId);

            if (startDate.HasValue)
                query = query.Where(t => t.UpdatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.UpdatedAt <= endDate.Value);

            return await query
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }
        public async Task<Transaction> GetTransactionDetailsAsync(int transactionId)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }
        public async Task<bool> CancelTransactionAsync(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null || transaction.Status == "Success")
                return false;

            transaction.Status = "Canceled";
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<string> GetTransactionStatusAsync(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            return transaction?.Status ?? "Not Found";
        }

        public async Task<string> InitiateWithdrawAsync(int accountId, decimal amount)
        {
            var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return "Account not found.";
            if (amount <= 0) return "Withdrawal amount must be positive.";
            if (account.Balance < amount) return "Insufficient balance.";

            var otp = _otpService.GenerateOTP();
            account.User.OTP = otp;
            account.User.OTPGeneratedAt = DateTime.Now;

            var transaction = new Transaction
            {
                AccountID = accountId,
                Amount = amount,
                Status = "Pending",
                TransactionType = "Withdraw",
                UpdatedAt = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(account.User.Email, otp, transaction.Id);

            return $"OTP sent for withdraw. Transaction ID: {transaction.Id}";
        }
        public async Task<(bool success, string message, decimal? newBalance)> ConfirmWithdrawAsync(int accountId, decimal amount, string otp)
        {
            var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return (false, "Account not found.", null);

            if (!_otpService.ValidateOTP(otp, account.User.OTP, account.User.OTPGeneratedAt ?? DateTime.MinValue))
                return (false, "Invalid or expired OTP.", null);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                account.Balance -= amount;

                var completedTransaction = new Transaction
                {
                    AccountID = accountId,
                    Amount = amount,
                    Status = "Success",
                    TransactionType = "Withdraw",
                    UpdatedAt = DateTime.Now
                };

                _context.Transactions.Add(completedTransaction);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, "Withdrawal successful.", account.Balance);
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Transaction failed.", null);
            }
        }
        public async Task<string> InitiateDepositAsync(int accountId, decimal amount)
        {
            var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return "Account not found.";
            if (amount < 5) return "Minimum deposit is $5.";

            var otp = _otpService.GenerateOTP();

            var transaction = new Transaction
            {
                AccountID = accountId,
                Amount = amount,
                Status = "Pending",
                TransactionType = "Deposit",
                UpdatedAt = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            account.User.OTP = otp;
            account.User.OTPGeneratedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(account.User.Email, otp, transaction.Id);

            return $"OTP sent. Transaction ID: {transaction.Id}";
        }
        public async Task<string> ConfirmDepositAsync(int accountId, int transactionId, string otp)
        {
            var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null || account.User == null) return "Invalid account.";

            if (!_otpService.ValidateOTP(otp, account.User.OTP, account.User.OTPGeneratedAt ?? DateTime.MinValue))
                return "Invalid or expired OTP.";

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.AccountID == accountId && t.Status == "Pending");

            if (transaction == null) return "Transaction not found.";

            return "Awaiting customer service confirmation.";
        }
        public async Task<string> CustomerServiceConfirmDepositAsync(int transactionId, string otp)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Account).ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null) return "Transaction not found.";

            var account = transaction.Account;
            var user = account.User;

            if (!_otpService.ValidateOTP(otp, user.OTP, user.OTPGeneratedAt ?? DateTime.MinValue))
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
                return "OTP invalid or expired. Transaction canceled.";
            }

            account.Balance += transaction.Amount;
            transaction.Status = "Success";
            account.LastUpdatedAt = DateTime.Now;
            user.OTP = null;
            user.OTPGeneratedAt = null;

            await _context.SaveChangesAsync();
            return $"Deposit successful. New Balance: {account.Balance}";
        }

        public async Task<(bool success, string message)> TransferMoneyAsync(string fromAccountNumber, string toAccountNumber, decimal amount)
        {
            if (fromAccountNumber == toAccountNumber)
                return (false, "Cannot transfer to the same account.");

            var fromAccount = await _context.Accounts.Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == fromAccountNumber);
            var toAccount = await _context.Accounts.Include(a => a.User)
                .FirstOrDefaultAsync(a => a.AccountNumber == toAccountNumber);

            if (fromAccount == null)
                return (false, "Source account not found.");

            if (toAccount == null)
                return (false, "Destination account not found.");

            if (amount <= 0)
                return (false, "Transfer amount must be positive.");

            if (fromAccount.Balance < amount)
                return (false, "Insufficient balance.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // خصم من حساب المرسل
                fromAccount.Balance -= amount;
                fromAccount.LastUpdatedAt = DateTime.Now;

                var fromTransaction = new Transaction
                {
                    AccountID = fromAccount.Id,
                    Amount = amount,
                    Status = "Success",
                    TransactionType = "TransferOut",
                    UpdatedAt = DateTime.Now
                };
                _context.Transactions.Add(fromTransaction);

                // إضافة لحساب المستلم
                toAccount.Balance += amount;
                toAccount.LastUpdatedAt = DateTime.Now;

                var toTransaction = new Transaction
                {
                    AccountID = toAccount.Id,
                    Amount = amount,
                    Status = "Success",
                    TransactionType = "TransferIn",
                    UpdatedAt = DateTime.Now
                };
                _context.Transactions.Add(toTransaction);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return (true, $"Transfer of {amount} successfully from {fromAccountNumber} to {toAccountNumber}.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, $"Transaction failed: {ex.Message}");
            }
        }


    }
}
