namespace BankSystem.Data.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;
        public string? Status { get; set; }
        public decimal Balance { get; set; } = 0;
        public int UserID { get; set; }
        public User User { get; set; }
        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    }
}
