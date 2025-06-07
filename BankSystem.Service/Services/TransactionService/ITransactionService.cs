using BankSystem.Data.Entities;

namespace BankSystem.Service.Services.TransactionService
{
    public interface ITransactionService
    {
        Task<List<Transaction>> GetTransactionHistoryAsync(int accountId, DateTime? startDate = null, DateTime? endDate = null);
        Task<Transaction> GetTransactionDetailsAsync(int transactionId);
        Task<bool> CancelTransactionAsync(int transactionId);
        Task<string> GetTransactionStatusAsync(int transactionId);


        Task<string> InitiateWithdrawAsync(int accountId, decimal amount);
        Task<(bool success, string message, decimal? newBalance)> ConfirmWithdrawAsync(int accountId, decimal amount, string otp);


        Task<string> InitiateDepositAsync(int accountId, decimal amount);
        Task<string> ConfirmDepositAsync(int accountId, int transactionId, string otp);
        Task<string> CustomerServiceConfirmDepositAsync(int transactionId, string otp);


        Task<string> InitiateTransferFundsAsync(int senderAccountId, string receiverAccountNumber, decimal amount);
        Task<string> ConfirmTransferFundsAsync(int senderAccountId, string receiverAccountNumber, decimal amount, string otp);
    }
}