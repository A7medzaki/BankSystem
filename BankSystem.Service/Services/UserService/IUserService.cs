using BankSystem.Data.Entities;

namespace BankSystem.Service.Services.UserService
{
    public interface IUserService
    {
        Task<string> RegisterUserAsync(string email, string password, string name);
        Task<User> GetUserByIdAsync(int userId);
        Task<User> GetUserWithAccountAsync(int userId);
        Task<string> UpdateUserAsync(int userId, string email, string name);
        Task<string> DeleteUserAsync(int userId);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
