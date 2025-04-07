using BankSystem.Data.Entities;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByAccountNumberAsync(string accountNumber);
        Task<Account> GetAccountByUserIdAsync(int userId);
        Task<bool> AccountExistsAsync(string accountNumber);
    }
}
