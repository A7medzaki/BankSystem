using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Repository.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        private readonly BankingContext _context;

        public AccountRepository(BankingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Account> GetByIdWithUserAsync(int id)
        {
            return await _context.Accounts
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);
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

        public async Task<Account> GetByIdAsync(int id, bool includeUser = false)
        {
            IQueryable<Account> query = _context.Accounts;

            if (includeUser)
                query = query.Include(a => a.User);

            return await query.FirstOrDefaultAsync(a => a.Id == id);
        }

        
    }
}
