using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Status { get; set; }
        public string? TransactionType { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int AccountID { get; set; }
        public Account Account { get; set; }
    }
}
