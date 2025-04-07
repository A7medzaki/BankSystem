using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using Microsoft.AspNetCore.Identity;

namespace BankSystem.Service.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IAccountRepository accountRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<string> RegisterUserAsync(string email, string password, string name)
        {
            var existingUser = await _userRepository.GetByEmailAsync(email);
            if (existingUser != null)
            {
                return "User already exists.";
            }

            string hashedPassword = _passwordHasher.HashPassword(new User(), password);

            var user = new User
            {
                Email = email,
                Name = name,
                HashedPassword = hashedPassword
            };

            await _userRepository.AddAsync(user);

            var account = new Account
            {
                UserID = user.Id,
                Balance = 0
            };
            await _accountRepository.AddAsync(account);

            return "User registered successfully.";
        }
        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            return user;
        }
        public async Task<User> GetUserWithAccountAsync(int userId)
        {
            var user = await _userRepository.GetUserWithAccountAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            return user;
        }
        public async Task<string> UpdateUserAsync(int userId, string email, string name)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            user.Email = email;
            user.Name = name;

            await _userRepository.UpdateAsync(user);

            return "User updated successfully.";
        }
        public async Task<string> DeleteUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            var account = await _accountRepository.GetAccountByUserIdAsync(userId);
            if (account != null)
            {
                await _accountRepository.DeleteAsync(account.Id);
            }

            await _userRepository.DeleteAsync(userId);

            return "User deleted successfully.";
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }
    }

}
