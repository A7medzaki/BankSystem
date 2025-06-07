using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using BankSystem.Service.Services.SubscriptionService.DTOs;

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

        public async Task<string> CreateSubscriptionAsync(SubscriptionDto subscriptionDto)
        {
            try
            {
                if (subscriptionDto.PartnerId.HasValue)
                {
                    var partner = await _partnerRepository.GetByIdAsync(subscriptionDto.PartnerId.Value);
                    if (partner == null)
                        return "Partner not found.";
                }

                var account = await _accountRepository.GetByIdAsync(subscriptionDto.AccountId);
                if (account == null)
                    return "Account not found.";

                var subscription = new Subscription
                {
                    SubscriptionName = subscriptionDto.SubscriptionName,
                    Amount = subscriptionDto.Amount,
                    Discount = subscriptionDto.Discount,
                    StartDate = subscriptionDto.StartDate,
                    EndDate = subscriptionDto.EndDate,
                    RenewalDate = subscriptionDto.RenewalDate,
                    IsActive = subscriptionDto.IsActive,
                    AccountId = subscriptionDto.AccountId,
                    PartnerId = subscriptionDto.PartnerId ?? 0
                };

                await _subscriptionRepository.AddAsync(subscription);
                return "Subscription created successfully.";
            }
            catch (Exception ex)
            {
                return $"Failed to create subscription: {ex.Message}";
            }
        }

        public async Task<string> UpdateSubscriptionAsync(int subscriptionId, SubscriptionDto subscriptionDto)
        {
            try
            {
                var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);
                if (subscription == null)
                    return "Subscription not found.";

                if (subscriptionDto.PartnerId.HasValue)
                {
                    var partner = await _partnerRepository.GetByIdAsync(subscriptionDto.PartnerId.Value);
                    if (partner == null)
                        return "Partner not found.";
                }

                var account = await _accountRepository.GetByIdAsync(subscriptionDto.AccountId);
                if (account == null)
                    return "Account not found.";

                subscription.SubscriptionName = subscriptionDto.SubscriptionName;
                subscription.Amount = subscriptionDto.Amount;
                subscription.Discount = subscriptionDto.Discount;
                subscription.StartDate = subscriptionDto.StartDate;
                subscription.EndDate = subscriptionDto.EndDate;
                subscription.RenewalDate = subscriptionDto.RenewalDate;
                subscription.IsActive = subscriptionDto.IsActive;
                subscription.AccountId = subscriptionDto.AccountId;
                subscription.PartnerId = subscriptionDto.PartnerId ?? 0;

                await _subscriptionRepository.UpdateAsync(subscription);
                return "Subscription updated successfully.";
            }
            catch (Exception ex)
            {
                return $"Failed to update subscription: {ex.Message}";
            }
        }

        public async Task<string> DeleteSubscriptionAsync(int subscriptionId)
        {
            try
            {
                var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);
                if (subscription == null)
                    return "Subscription not found.";

                subscription.IsActive = false;
                await _subscriptionRepository.UpdateAsync(subscription);

                return "Subscription deactivated successfully.";
            }
            catch (Exception ex)
            {
                return $"Failed to delete subscription: {ex.Message}";
            }
        }

        public async Task<SubscriptionDto> GetSubscriptionByIdAsync(int subscriptionId)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(subscriptionId);
            if (subscription == null) return null;

            return new SubscriptionDto
            {
                Id = subscription.Id,
                SubscriptionName = subscription.SubscriptionName,
                Amount = subscription.Amount,
                Discount = subscription.Discount,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                RenewalDate = subscription.RenewalDate,
                IsActive = subscription.IsActive,
                AccountId = subscription.AccountId,
                PartnerId = subscription.PartnerId
            };
        }

        public async Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync(bool onlyActive = true)


        {
            var subscriptions = await _subscriptionRepository.GetAllAsync();

            if (onlyActive)
                subscriptions = subscriptions.Where(s => s.IsActive);

            return subscriptions.Select(subscription => new SubscriptionDto
            {
                Id = subscription.Id,
                SubscriptionName = subscription.SubscriptionName,
                Amount = subscription.Amount,
                Discount = subscription.Discount,
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                RenewalDate = subscription.RenewalDate,
                IsActive = subscription.IsActive,
                AccountId = subscription.AccountId,
                PartnerId = subscription.PartnerId
            });
        }
    }
}