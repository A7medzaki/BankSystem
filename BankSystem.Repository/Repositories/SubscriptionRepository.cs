using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Repository.Repositories
{
    public class SubscriptionRepository : GenericRepository<Subscription>, ISubscriptionRepository
    {
        public SubscriptionRepository(BankingContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Subscription>> GetActiveSubscriptionsByAccountIdAsync(int accountId)
        {
            return await _context.Subscriptions
                                 .Where(s => s.AccountId == accountId && s.IsActive)
                                 .ToListAsync();
        }
    }

}
