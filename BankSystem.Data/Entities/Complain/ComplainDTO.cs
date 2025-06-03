using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Data.Entities
{
    public class CreateComplainDto
    {
        public string? Describtion { get; set; }
        public string? Recipient { get; set; }
        public bool Solved { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime? EndDate { get; set; }
        public int UserId { get; set; } 
    }

}
