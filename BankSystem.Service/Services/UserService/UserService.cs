using BankSystem.Data.Entities;
using BankSystem.Data.Repositories;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Services.UserService.BankSystem.Service.Services.UserService;

namespace BankSystem.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync() => await _repository.GetAllAsync();

        public async Task<User?> GetUserByIdAsync(int id) => await _repository.GetByIdAsync(id);

        public async Task<bool> CreateUserAsync(User user)
        {
            await _repository.AddAsync(user);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAsync(int id, User updatedUser)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.UserName = updatedUser.UserName;
            existing.Email = updatedUser.Email;
            existing.HashedPassword = updatedUser.HashedPassword;
            existing.PhoneNumber = updatedUser.PhoneNumber;

            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return false;

            await _repository.DeleteAsync(user);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<(bool, string)> CheckDuplicateAsync(string email, string? gmail = null, string? facebookId = null)
        {
            if (await _repository.GetByEmailAsync(email) is not null)
                return (true, "Email already exists.");

            if (!string.IsNullOrEmpty(gmail) && await _repository.GetByGmailAsync(gmail) is not null)
                return (true, "Gmail already registered.");

            if (!string.IsNullOrEmpty(facebookId) && await _repository.GetByFacebookIdAsync(facebookId) is not null)
                return (true, "Facebook ID already registered.");

            return (false, string.Empty);
        }
    }
}
