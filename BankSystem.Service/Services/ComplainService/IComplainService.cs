using BankSystem.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.ComplainService
{
    public interface IComplainService
    {
        Task<List<Complain>> GetAllComplainsAsync();
        Task<Complain?> GetComplainByIdAsync(int id);
        Task<Complain> CreateComplainAsync(Complain complain);
        Task<bool> UpdateComplainAsync(int id, Complain updatedComplain);
        Task<bool> DeleteComplainAsync(int id);
    }
}
