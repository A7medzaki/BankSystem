using BankSystem.Data.Entities;
using BankSystem.Repository.Repositories;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using BankSystem.Service.Services.AccountService;
using BankSystem.Service.Services.Security;
using BankSystem.Service.Services.SubscriptionService;
using BankSystem.Service.Services.TransactionService;
using BankSystem.Service.Services.UserService;
using Microsoft.AspNetCore.Identity;

namespace BankSystem.API.Extension
{
    public static class ApplicationServices
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
<<<<<<< Updated upstream
            services.AddScoped<IRepository<Account>, AccountRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IRepository<Transaction>, TransactionRepository>();
            services.AddScoped<IRepository<Subscription>, SubscriptionRepository>();
            services.AddScoped<IRepository<Partner>, PartnerRepository>();
            services.AddScoped<IRepository<User>, UserRepository>();
=======

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IPartnerRepository, PartnerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
>>>>>>> Stashed changes

            services.AddScoped<IAccountService, AccountService>();
            //services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            //services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            services.AddScoped<EmailService>();
            services.AddScoped<OTPService>();
            services.AddScoped<InterestService>();
            services.AddScoped<TaxService>();



        }
    }

}
