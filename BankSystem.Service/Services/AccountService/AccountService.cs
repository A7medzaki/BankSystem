using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
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
        public AccountService(BankingContext context, TaxService taxService, InterestService interestService, IAccountRepository accountRepository)
        {
            _context = context;
            _taxService = taxService;
            _interestService = interestService;
            _accountRepository = accountRepository;
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
