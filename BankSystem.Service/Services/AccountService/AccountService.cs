using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.Repositories;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Service.Services.AccountService
{
    public class AccountService : IAccountService
    {
        private readonly BankingContext _context;
        private readonly TaxService _taxService;
        private readonly InterestService _interestService;
        private readonly IAccountRepository _accountRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly OTPService _otpService;
        private readonly EmailService _emailService;

        public AccountService(BankingContext context, TaxService taxService, InterestService interestService, IAccountRepository accountRepository, ITransactionRepository transactionRepository, OTPService otpService, EmailService emailService)
        {
            _context = context;
            _taxService = taxService;
            _interestService = interestService;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
            _otpService = otpService;
            _emailService = emailService;
        }

        private string GenerateAccountNumber()
        {
            Random random = new Random();

            // Generate a 12-digit random number by combining two 6-digit numbers
            long part1 = random.Next(100000, 999999);
            long part2 = random.Next(100000, 999999);

            long accountNumber = part1 * 1000000 + part2;

            return accountNumber.ToString();
        }
        public async Task<Account> CreateAccountAsync(int userId)
        {
            var newAccount = new Account
            {
                UserID = userId,
                AccountNumber = GenerateAccountNumber(),
                Balance = 0
            };

            while (await _accountRepository.AccountExistsAsync(newAccount.AccountNumber))
            {
                newAccount.AccountNumber = GenerateAccountNumber();
            }

            // Save the new account
            await _accountRepository.AddAsync(newAccount);

            return newAccount;
        }
        public async Task<decimal> GetAccountBalanceAsync(int accountId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            return account.Balance;
        }
        public async Task<Account> GetAccountDetailsByIdAsync(int accountId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            return account;
        }
        public async Task<Account> GetAccountDetailsByUserIdAsync(int userId)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.UserID == userId);

            if (account == null)
            {
                throw new Exception("Account not found for the given user.");
            }

            return account;
        }
        public async Task<string> UpdateAccountDetailsAsync(int accountId, decimal newBalance, string accountStatus)
        {
            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            account.Balance = newBalance;
            account.Status = accountStatus;

            await _context.SaveChangesAsync();

            return "Account updated successfully.";
        }
        public async Task<string> UpdateAccountStatusAsync(int accountId, string status)
        {
            // Validate the status input
            if (status != "Active" && status != "Inactive")
            {
                throw new ArgumentException("Invalid status. Status must be either 'Active' or 'Inactive'.");
            }

            var account = await _context.Accounts
                .FirstOrDefaultAsync(a => a.Id == accountId);

            if (account == null)
            {
                throw new Exception("Account not found.");
            }

            // Update the account status based on the input
            account.Status = status;

            await _context.SaveChangesAsync();

            return $"Account has been successfully {status}.";
        }

        public async Task<string> InitiateTransferFundsAsync(int senderAccountId, string receiverAccountNumber, decimal amount)
        {
            var senderAccount = await _accountRepository.GetByIdAsync(senderAccountId);
            if (senderAccount == null)
            {
                return "Sender account not found.";
            }

            var receiverAccount = await _accountRepository.GetByAccountNumberAsync(receiverAccountNumber);
            if (receiverAccount == null)
            {
                return "Receiver account not found.";
            }

            if (senderAccount.Balance < amount)
            {
                return "Insufficient balance.";
            }

            var otp = _otpService.GenerateOTP();
            senderAccount.User.OTP = otp;
            senderAccount.User.OTPGeneratedAt = DateTime.Now;

            var transaction = new Transaction
            {
                AccountID = senderAccountId,
                Amount = amount,
                Status = "Pending",
                TransactionType = "Transfer",
                UpdatedAt = DateTime.Now
            };

            await _transactionRepository.AddAsync(transaction);

            // Send OTP to user for validation
            await _emailService.SendEmailAsync(senderAccount.User.Email, otp, transaction.Id);

            return $"OTP sent for transfer. Transaction ID: {transaction.Id}";
        }
        public async Task<string> ConfirmTransferFundsAsync(int senderAccountId, string receiverAccountNumber, decimal amount, string otp)
        {
            // Retrieve sender account
            var senderAccount = await _accountRepository.GetByIdAsync(senderAccountId);
            if (senderAccount == null)
            {
                return "Sender account not found.";
            }

            // Retrieve receiver account by account number
            var receiverAccount = await _accountRepository.GetByAccountNumberAsync(receiverAccountNumber);
            if (receiverAccount == null)
            {
                return "Receiver account not found.";
            }

            // Validate OTP
            if (!_otpService.ValidateOTP(otp, senderAccount.User.OTP, senderAccount.User.OTPGeneratedAt ?? DateTime.MinValue))
            {
                return "Invalid or expired OTP.";
            }

            // Begin a database transaction for atomic operations
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Deduct the amount from the sender's account
                senderAccount.Balance -= amount;

                // Add the amount to the receiver's account
                receiverAccount.Balance += amount;

                // Create a successful transaction record
                var completedTransaction = new Transaction
                {
                    AccountID = senderAccountId,  // Store sender's account ID
                    Amount = amount,
                    Status = "Success",  // Mark transaction as successful
                    TransactionType = "Transfer",
                    UpdatedAt = DateTime.Now
                };

                // Save the completed transaction
                await _transactionRepository.AddAsync(completedTransaction);

                // Save the updated accounts (both sender and receiver)
                await _accountRepository.UpdateAsync(senderAccount);
                await _accountRepository.UpdateAsync(receiverAccount);

                // Commit the database transaction to persist changes
                await transaction.CommitAsync();

                // Clear the OTP after successful transfer
                senderAccount.User.OTP = null;
                senderAccount.User.OTPGeneratedAt = null;
                await _accountRepository.UpdateAsync(senderAccount);

                return "Transfer successful.";
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error
                await transaction.RollbackAsync();
                return $"Transfer failed: {ex.Message}";
            }
        }

        public async Task<decimal?> ApplyMonthlyTaxAsync(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return null;
            if (account.Balance < 10000) return account.Balance;

            var newBalance = _taxService.ApplyMonthlyTax(account.Balance);
            account.Balance = newBalance;
            account.LastUpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return account.Balance;
        }
        public async Task<decimal?> ApplyAnnualTaxAsync(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return null;
            if (account.Balance < 10000) return account.Balance;

            var newBalance = _taxService.ApplyAnnualTax(account.Balance);
            account.Balance = newBalance;
            account.LastUpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return account.Balance;
        }
        public async Task<decimal?> ApplyMonthlyInterestAsync(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return null;

            var newBalance = _interestService.ApplyMonthlyInterest(account.Balance);
            account.Balance = newBalance;
            account.LastUpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return account.Balance;
        }
        public async Task<decimal?> ApplyAnnualInterestAsync(int accountId)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return null;

            var newBalance = _interestService.ApplyAnnualInterest(account.Balance);
            account.Balance = newBalance;
            account.LastUpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            return account.Balance;
        }
    }

}
