namespace BankSystem.Data.Entities
{
    public class Complain
    {
        public int Id { get; set; }
        public string? Describtion { get; set; }
        public string? Recipient { get; set; }
        public bool Solved { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime? EndDate { get; set; }
        public int UserId { get; set; }

        public User User { get; set; } = null!;
    }
}
