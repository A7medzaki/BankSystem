using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Repository.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private readonly BankingContext _context;

        public UserRepository(BankingContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        }
        public async Task<User> GetUserWithAccountAsync(int userId)
        {
            return await _context.Users.Include(u => u.Account).FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
