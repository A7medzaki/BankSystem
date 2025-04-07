namespace BankSystem.Service.Services.SubscriptionService
{
    public interface ISubscriptionService
    {
        Task<bool> CreateSubscriptionAsync(int accountId, int partnerId, decimal amount, decimal discount = 0);
        Task ProcessSubscriptionsAsync();
        Task CancelExpiredSubscriptionsAsync();
    }
}
