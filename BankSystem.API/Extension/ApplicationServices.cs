using BankSystem.Data.Entities;
using BankSystem.Repository.Repositories;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using BankSystem.Service.Services.AccountService;
using BankSystem.Service.Services.Security;
using BankSystem.Service.Services.SubscriptionService;
using BankSystem.Service.Services.TransactionService;
using BankSystem.Service.Services.UserService;

namespace BankSystem.API.Extension
{
    public static class ApplicationServices
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Account>, AccountRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IRepository<Transaction>, TransactionRepository>();
            services.AddScoped<IRepository<Subscription>, SubscriptionRepository>();
            services.AddScoped<IRepository<Partner>, PartnerRepository>();
            services.AddScoped<IRepository<User>, UserRepository>();

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IUserService, UserService>();

            services.AddScoped<IPasswordHasherService, PasswordHasherService>();

            services.AddScoped<EmailService>();
            services.AddScoped<OTPService>();
            services.AddScoped<InterestService>();
            services.AddScoped<TaxService>();

        }
    }

}
