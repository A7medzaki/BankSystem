using BankSystem.Data.Entities;

namespace BankSystem.Repository.RepositoryInterfaces
{
    public interface IAccountRepository : IRepository<Account>
    {
        Task<Account> GetByAccountNumberAsync(string accountNumber);
        Task<Account> GetByIdWithUserAsync(int userId);
        Task<bool> AccountExistsAsync(string accountNumber);

    }
}
