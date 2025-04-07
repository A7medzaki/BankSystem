using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;

namespace BankSystem.Service.Services.AccountService
{
    public interface IAccountService
    {
        Task<Account> CreateAccountAsync(int userId);
        Task<decimal> GetAccountBalanceAsync(int accountId);
        Task<Account> GetAccountDetailsByIdAsync(int accountId);
        Task<Account> GetAccountDetailsByUserIdAsync(int userId);
        Task<string> UpdateAccountDetailsAsync(int accountId, decimal newBalance, string accountStatus);
        Task<string> UpdateAccountStatusAsync(int accountId, string status);

        Task<decimal?> ApplyMonthlyTaxAsync(int accountId);
        Task<decimal?> ApplyAnnualTaxAsync(int accountId);
        Task<decimal?> ApplyMonthlyInterestAsync(int accountId);
        Task<decimal?> ApplyAnnualInterestAsync(int accountId);
    }

}
