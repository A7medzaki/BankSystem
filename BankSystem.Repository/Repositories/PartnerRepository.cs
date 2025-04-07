using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Repository.Repositories
{
    public class PartnerRepository : GenericRepository<Partner>, IPartnerRepository
    {
        public PartnerRepository(BankingContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Partner>> GetPartnersWithActiveSubscriptionsAsync()
        {
            return await _context.Partners
                                 .Where(p => p.Subscriptions.Any(s => s.IsActive))
                                 .ToListAsync();
        }
    }
}
