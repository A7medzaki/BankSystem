using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Repository.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(BankingContext context) : base(context)
        {
        }

        public async Task<Account> GetAccountByUserIdAsync(int userId)
        {
            return await _context.Accounts.Include(a => a.Transactions)
                                          .Include(a => a.Subscriptions)
                                          .FirstOrDefaultAsync(a => a.UserID == userId);
        }

        public async Task<bool> AccountExistsAsync(string accountNumber)
        {
            return await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber);
        }

        public async Task<Account> GetByAccountNumberAsync(string accountNumber)
        {
            return await _context.Accounts
                .FirstOrDefaultAsync(a => a.AccountNumber == accountNumber);
        }
    }
}
