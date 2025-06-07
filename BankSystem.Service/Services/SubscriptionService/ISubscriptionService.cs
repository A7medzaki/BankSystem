using BankSystem.Data.Entities;
using BankSystem.Service.Services.SubscriptionService.DTOs;

namespace BankSystem.Service.Services.SubscriptionService
{
    public interface ISubscriptionService
    {
        Task<string> CreateSubscriptionAsync(SubscriptionDto subscriptionDto);
        Task<string> UpdateSubscriptionAsync(int subscriptionId, SubscriptionDto subscriptionDto);
        Task<string> DeleteSubscriptionAsync(int subscriptionId);
        Task<SubscriptionDto> GetSubscriptionByIdAsync(int subscriptionId);
        Task<IEnumerable<SubscriptionDto>> GetAllSubscriptionsAsync(bool onlyActive = true);
    }
}
