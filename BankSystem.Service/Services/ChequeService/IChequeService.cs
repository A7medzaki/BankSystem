using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.CheckService
{
    public interface IChequeService
    {
        Task<byte[]> GenerateChequePdfAsync(string fromAccountName, string toName, decimal amount, string chequeNumber);
    }
}
