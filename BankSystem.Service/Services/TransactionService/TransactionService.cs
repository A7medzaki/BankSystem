using BankSystem.Data.Contexts;
using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using BankSystem.Service.Helper;
using BankSystem.Service.Services.ReportService;
using Microsoft.EntityFrameworkCore;

namespace BankSystem.Service.Services.TransactionService
{
    public class TransactionService : ITransactionService
    {
        private readonly BankingContext _context;
        private readonly OTPService _otpService;
        private readonly EmailService _emailService;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IReportService _reportService;

        public TransactionService(BankingContext context, OTPService otpService, EmailService emailService, ITransactionRepository transactionRepository, IAccountRepository accountRepository, IReportService reportService)
        {
            _context = context;
            _otpService = otpService;
            _emailService = emailService;
            _transactionRepository = transactionRepository;
            _accountRepository = accountRepository;
            _reportService = reportService;
        }

        public async Task<List<Transaction>> GetTransactionHistoryAsync(int accountId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Transactions
                .Where(t => t.AccountID == accountId);

            if (startDate.HasValue)
                query = query.Where(t => t.UpdatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(t => t.UpdatedAt <= endDate.Value);

            return await query
                .OrderByDescending(t => t.UpdatedAt)
                .ToListAsync();
        }
        public async Task<Transaction> GetTransactionDetailsAsync(int transactionId)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId);
        }
        public async Task<bool> CancelTransactionAsync(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null || transaction.Status == "Success")
                return false;

            transaction.Status = "Canceled";
            transaction.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<string> GetTransactionStatusAsync(int transactionId)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            return transaction?.Status ?? "Not Found";
        }

        
        public async Task<string> InitiateWithdrawAsync(int accountId, decimal amount)
        {
            var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return "Account not found.";
            if (amount <= 0) return "Withdrawal amount must be positive.";
            if (account.Balance < amount) return "Insufficient balance.";

            var otp = _otpService.GenerateOTP();
            account.User.OTP = otp;
            account.User.OTPGeneratedAt = DateTime.Now;

            var transaction = new Transaction
            {
                AccountID = accountId,
                Amount = amount,
                Status = "Pending",
                TransactionType = "Withdraw",
                UpdatedAt = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(account.User.Email, otp, transaction.Id);

            return $"OTP sent for withdraw. Transaction ID: {transaction.Id}";
        }
        public async Task<(bool success, string message, decimal? newBalance)> ConfirmWithdrawAsync(int accountId, decimal amount, string otp)
        {
            var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return (false, "Account not found.", null);

            if (!_otpService.ValidateOTP(otp, account.User.OTP, account.User.OTPGeneratedAt ?? DateTime.MinValue))
                return (false, "Invalid or expired OTP.", null);

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                account.Balance -= amount;

                var completedTransaction = new Transaction
                {
                    AccountID = accountId,
                    Amount = amount,
                    Status = "Success",
                    TransactionType = "Withdraw",
                    UpdatedAt = DateTime.Now
                };

                _context.Transactions.Add(completedTransaction);

                var reference = Guid.NewGuid().ToString().Substring(0, 8);

                var reportDto = new TransactionReport
                {
                    UserFullName = account.User.UserName,
                    AccountNumber = account.AccountNumber,
                    TransactionType = "Withdraw",
                    Amount = amount,
                    Date = DateTime.Now,
                    ReferenceNumber = reference,
                    Status = "Success"
                };

                var pdfBytes = _reportService.GenerateTransactionReceiptPdf(reportDto);

                var reportHistory = new ReportHistory
                {
                    AccountId = account.Id,
                    TransactionType = "Withdraw",
                    Amount = amount,
                    Date = DateTime.Now,
                    ReferenceNumber = reference,
                    Status = "Success",
                    PdfBytes = pdfBytes
                };

                _context.ReportHistories.Add(reportHistory);


                await _context.SaveChangesAsync();
                await transaction.CommitAsync();


                var subject = "Withdrawal Receipt - STC Bank";
                var body = $"Dear {account.User.UserName},\n\nYour withdrawal of ${amount} was successful. Please find your receipt attached.\n\nThank you for banking with us.";
                var attachmentName = $"WithdrawReceipt{reference}.pdf";

                await _emailService.SendEmailWithAttachmentAsync(account.User.Email, subject, body, pdfBytes, attachmentName);
                return (true, "Withdrawal successful.", account.Balance);
            }
            catch
            {
                await transaction.RollbackAsync();
                return (false, "Transaction failed.", null);
            }
        }
       
        
        public async Task<string> InitiateDepositAsync(int accountId, decimal amount)
        {
            var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return "Account not found.";
            if (amount < 5) return "Minimum deposit is $5.";

            var otp = _otpService.GenerateOTP();

            var transaction = new Transaction
            {
                AccountID = accountId,
                Amount = amount,
                Status = "Pending",
                TransactionType = "Deposit",
                UpdatedAt = DateTime.Now
            };

            _context.Transactions.Add(transaction);
            account.User.OTP = otp;
            account.User.OTPGeneratedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            await _emailService.SendEmailAsync(account.User.Email, otp, transaction.Id);

            return $"OTP sent. Transaction ID: {transaction.Id}";
        }
        public async Task<string> ConfirmDepositAsync(int accountId, int transactionId, string otp)
        {
            var account = await _context.Accounts.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null || account.User == null) return "Invalid account.";

            if (!_otpService.ValidateOTP(otp, account.User.OTP, account.User.OTPGeneratedAt ?? DateTime.MinValue))
                return "Invalid or expired OTP.";

            var transaction = await _context.Transactions
                .FirstOrDefaultAsync(t => t.Id == transactionId && t.AccountID == accountId && t.Status == "Pending");

            if (transaction == null) return "Transaction not found.";

            return "Awaiting customer service confirmation.";
        }
        public async Task<string> CustomerServiceConfirmDepositAsync(int transactionId, string otp)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Account).ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null) return "Transaction not found.";

            var account = transaction.Account;
            var user = account.User;

            if (!_otpService.ValidateOTP(otp, user.OTP, user.OTPGeneratedAt ?? DateTime.MinValue))
            {
                _context.Transactions.Remove(transaction);
                await _context.SaveChangesAsync();
                return "OTP invalid or expired. Transaction canceled.";
            }

            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                account.Balance += transaction.Amount;
                account.LastUpdatedAt = DateTime.Now;

                transaction.Status = "Success";
                user.OTP = null;
                user.OTPGeneratedAt = null;

                var reference = Guid.NewGuid().ToString().Substring(0, 8);

                var reportDto = new TransactionReport
                {
                    UserFullName = user.UserName,
                    AccountNumber = account.AccountNumber,
                    TransactionType = "Deposit",
                    Amount = transaction.Amount,
                    Date = DateTime.Now,
                    ReferenceNumber = reference,
                    Status = "Success"
                };

                var pdfBytes = _reportService.GenerateTransactionReceiptPdf(reportDto);

                var reportHistory = new ReportHistory
                {
                    AccountId = account.Id,
                    TransactionType = "Deposit",
                    Amount = transaction.Amount,
                    Date = DateTime.Now,
                    ReferenceNumber = reference,
                    Status = "Success",
                    PdfBytes = pdfBytes
                };

                _context.ReportHistories.Add(reportHistory);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                var subject = "Deposit Receipt - STC Bank";
                var body = $"Dear {user.UserName},\n\nYour deposit of ${transaction.Amount} was successful. Please find your receipt attached.\n\nThank you for banking with us.";
                var attachmentName = $"DepositReceipt{reference}.pdf";

                await _emailService.SendEmailWithAttachmentAsync(user.Email, subject, body, pdfBytes, attachmentName);


                return $"Deposit successful. New Balance: {account.Balance}";
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                Console.WriteLine($"Error: {ex.Message}");
                return $"Transaction failed. Reason: {ex.Message}";
            }
        }


        public async Task<string> InitiateTransferFundsAsync(int senderAccountId, string receiverAccountNumber, decimal amount)
        {
            try
            {
                var senderAccount = await _accountRepository.GetByIdWithUserAsync(senderAccountId);
                if (senderAccount == null)
                    return "Sender account not found.";

                var receiverAccount = await _accountRepository.GetByAccountNumberAsync(receiverAccountNumber);
                if (receiverAccount == null)
                    return "Receiver account not found.";

                receiverAccount = await _accountRepository.GetByIdWithUserAsync(receiverAccount.Id);

                if (amount <= 0)
                    return "Transfer amount must be positive.";

                if (senderAccount.Balance < amount)
                    return "Insufficient balance.";

                var otp = _otpService.GenerateOTP();
                senderAccount.User.OTP = otp;
                senderAccount.User.OTPGeneratedAt = DateTime.UtcNow;

                var transaction = new Transaction
                {
                    AccountID = senderAccountId,
                    Amount = amount,
                    Status = "Pending",
                    TransactionType = "Transfer",
                    UpdatedAt = DateTime.UtcNow
                };

                await _transactionRepository.AddAsync(transaction);
                await _accountRepository.UpdateAsync(senderAccount);

                await _emailService.SendEmailAsync(senderAccount.User.Email, otp, transaction.Id);

                return $"OTP sent for transfer. Transaction ID: {transaction.Id}";
            }
            catch (Exception ex)
            {
                return $"Failed to initiate transfer: {ex.Message}";
            }
        }

        public async Task<string> ConfirmTransferFundsAsync(int senderAccountId, string receiverAccountNumber, decimal amount, string otp)
        {
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var senderAccount = await _accountRepository.GetByIdWithUserAsync(senderAccountId);
                if (senderAccount == null)
                    return "Sender account not found.";

                var receiverAccount = await _accountRepository.GetByAccountNumberAsync(receiverAccountNumber);
                if (receiverAccount == null)
                    return "Receiver account not found.";

                receiverAccount = await _accountRepository.GetByIdWithUserAsync(receiverAccount.Id);

                if (amount <= 0)
                    return "Transfer amount must be positive.";

                if (!_otpService.ValidateOTP(otp, senderAccount.User.OTP, senderAccount.User.OTPGeneratedAt ?? DateTime.MinValue))
                    return "Invalid or expired OTP.";

                if (senderAccount.Balance < amount)
                    return "Insufficient balance.";

             
                senderAccount.Balance -= amount;
                senderAccount.LastUpdatedAt = DateTime.UtcNow;

                receiverAccount.Balance += amount;
                receiverAccount.LastUpdatedAt = DateTime.UtcNow;

                var fromReference = Guid.NewGuid().ToString("N").Substring(0, 8);
                var toReference = Guid.NewGuid().ToString("N").Substring(0, 8);

                var completedTransaction = new Transaction
                {
                    AccountID = senderAccountId,
                    Amount = amount,
                    Status = "Success",
                    TransactionType = "TransferOut",
                    UpdatedAt = DateTime.UtcNow
                };
                await _transactionRepository.AddAsync(completedTransaction);

                var receivedTransaction = new Transaction
                {
                    AccountID = receiverAccount.Id,
                    Amount = amount,
                    Status = "Success",
                    TransactionType = "TransferIn",
                    UpdatedAt = DateTime.UtcNow
                };
                await _transactionRepository.AddAsync(receivedTransaction);

                
                var fromReport = new TransactionReport
                {
                    UserFullName = senderAccount.User.UserName,
                    AccountNumber = senderAccount.AccountNumber,
                    TransactionType = "TransferOut",
                    Amount = amount,
                    Date = DateTime.UtcNow,
                    ReferenceNumber = fromReference,
                    Status = "Success"
                };
                var fromPdf = _reportService.GenerateTransactionReceiptPdf(fromReport);

                var toReport = new TransactionReport
                {
                    UserFullName = receiverAccount.User.UserName,
                    AccountNumber = receiverAccount.AccountNumber,
                    TransactionType = "TransferIn",
                    Amount = amount,
                    Date = DateTime.UtcNow,
                    ReferenceNumber = toReference,
                    Status = "Success"
                };
                var toPdf = _reportService.GenerateTransactionReceiptPdf(toReport);

              
                _context.ReportHistories.Add(new ReportHistory
                {
                    AccountId = senderAccount.Id,
                    TransactionType = "TransferOut",
                    Amount = amount,
                    Date = DateTime.UtcNow,
                    ReferenceNumber = fromReference,
                    Status = "Success",
                    PdfBytes = fromPdf
                });

                _context.ReportHistories.Add(new ReportHistory
                {
                    AccountId = receiverAccount.Id,
                    TransactionType = "TransferIn",
                    Amount = amount,
                    Date = DateTime.UtcNow,
                    ReferenceNumber = toReference,
                    Status = "Success",
                    PdfBytes = toPdf
                });

                await _accountRepository.UpdateAsync(senderAccount);
                await _accountRepository.UpdateAsync(receiverAccount);

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

              
                senderAccount.User.OTP = null;
                senderAccount.User.OTPGeneratedAt = null;
                await _accountRepository.UpdateAsync(senderAccount);

              
                var subjectSender = "Transfer Receipt - STC Bank";
                var bodySender = $"Dear {senderAccount.User.UserName},\n\nYour transfer of ${amount} to account {receiverAccount.AccountNumber} was successful. Please find your receipt attached.\n\nThank you for banking with us.";
                var attachmentNameSender = $"TransferOut_Receipt_{fromReference}.pdf";

                await _emailService.SendEmailWithAttachmentAsync(senderAccount.User.Email, subjectSender, bodySender, fromPdf, attachmentNameSender);

                var subjectReceiver = "Transfer Receipt - STC Bank";
                var bodyReceiver = $"Dear {receiverAccount.User.UserName},\n\nYou have received a transfer of ${amount} from account {senderAccount.AccountNumber}. Please find your receipt attached.\n\nThank you for banking with us.";
                var attachmentNameReceiver = $"TransferIn_Receipt_{toReference}.pdf";

                await _emailService.SendEmailWithAttachmentAsync(receiverAccount.User.Email, subjectReceiver, bodyReceiver, toPdf, attachmentNameReceiver);

                return "Transfer successful.";
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return $"Transfer failed: {ex.Message}";
            }
        }

    }
}
