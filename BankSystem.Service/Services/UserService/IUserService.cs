
using BankSystem.Data.Entities;

namespace BankSystem.Service.Services.UserService
{


     namespace BankSystem.Service.Services.UserService
    {
        public interface IUserService
        {
            Task<IEnumerable<User>> GetAllUsersAsync();
            Task<User?> GetUserByIdAsync(int id);
            Task<bool> CreateUserAsync(User user);
            Task<bool> UpdateUserAsync(int id, User updatedUser);
            Task<bool> DeleteUserAsync(int id);
            Task<(bool exists, string reason)> CheckDuplicateAsync(string email, string? gmail = null, string? facebookId = null);
        }
    }

}

