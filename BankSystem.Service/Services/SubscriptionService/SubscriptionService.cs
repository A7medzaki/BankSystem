using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.Repositories;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;

namespace BankSystem.Service.Services.SubscriptionService
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly BankingContext _context;
        private readonly EmailService _emailService;
        private readonly IAccountRepository _accountRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPartnerRepository _partnerRepository;

        public SubscriptionService(BankingContext context, EmailService emailService, IAccountRepository accountRepository, ISubscriptionRepository subscriptionRepository, IPartnerRepository partnerRepository)
        {
            _context = context;
            _emailService = emailService;
            _accountRepository = accountRepository;
            _subscriptionRepository = subscriptionRepository;
            _partnerRepository = partnerRepository;
        }

        public async Task<string> AddOrUpdateSubscriptionAsync(int accountId, string subscriptionName, decimal amount, decimal discount, DateTime startDate, DateTime? endDate = null, int partnerId = 0)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
                return "Account not found.";

            // Validate partner only if it's not 0
            if (partnerId != 0)
            {
                var partner = await _partnerRepository.GetByIdAsync(partnerId);
                if (partner == null)
                    return "Partner not found.";
            }

            // Apply discount directly
            var finalAmount = amount - discount;
            if (finalAmount < 0) finalAmount = 0;

            var existingSubscription = await _subscriptionRepository.GetActiveSubscriptionByAccountIdAsync(accountId);

            if (existingSubscription != null)
            {
                existingSubscription.SubscriptionName = subscriptionName;
                existingSubscription.Amount = finalAmount;
                existingSubscription.Discount = discount;
                existingSubscription.StartDate = startDate;
                existingSubscription.EndDate = endDate ?? existingSubscription.EndDate;
                existingSubscription.PartnerId = partnerId;

                await _subscriptionRepository.UpdateAsync(existingSubscription);
                return $"Subscription updated successfully. Final amount: {finalAmount:C}";
            }
            else
            {
                var newSubscription = new Subscription
                {
                    AccountId = accountId,
                    SubscriptionName = subscriptionName,
                    Amount = finalAmount,
                    Discount = discount,
                    StartDate = startDate,
                    EndDate = endDate,
                    IsActive = true,
                    PartnerId = partnerId
                };

                await _subscriptionRepository.AddAsync(newSubscription);
                return $"Subscription added successfully. Final amount: {finalAmount:C}";
            }
        }

        public async Task<string> CancelSubscriptionAsync(int accountId)
        {
            var account = await _accountRepository.GetByIdAsync(accountId);
            if (account == null)
            {
                return "Account not found.";
            }

            var activeSubscription = await _subscriptionRepository.GetActiveSubscriptionByAccountIdAsync(accountId);
            if (activeSubscription == null)
            {
                return "No active subscription found for this account.";
            }

            activeSubscription.EndDate = DateTime.Now;

            await _subscriptionRepository.UpdateAsync(activeSubscription);
            return "Subscription cancelled successfully.";
        }
        public async Task<Subscription> GetSubscriptionByAccountIdAsync(int accountId)
        {
            return await _subscriptionRepository.GetActiveSubscriptionByAccountIdAsync(accountId);
        }
    }

}
