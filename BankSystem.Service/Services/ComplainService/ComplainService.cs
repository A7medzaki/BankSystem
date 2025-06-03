using BankSystem.Data.Entities;
using BankSystem.Repository.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankSystem.Service.Services.ComplainService
{
    public class ComplainService : IComplainService
    {
        private readonly IComplainRepository _repository;

        public ComplainService(IComplainRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Complain>> GetAllComplainsAsync() =>
            await _repository.GetAllAsync();

        public async Task<Complain?> GetComplainByIdAsync(int id) =>
            await _repository.GetByIdAsync(id);

        public async Task<Complain> CreateComplainAsync(Complain complain)
        {
            complain.Timestamp = DateTime.UtcNow;
            await _repository.AddAsync(complain);
            return complain;
        }

        public async Task<bool> UpdateComplainAsync(int id, Complain updatedComplain)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null) return false;

            existing.Describtion = updatedComplain.Describtion;
            existing.Recipient = updatedComplain.Recipient;
            existing.Solved = updatedComplain.Solved;
            existing.EndDate = updatedComplain.EndDate;

            await _repository.UpdateAsync(existing);
            return true;
        }

        public async Task<bool> DeleteComplainAsync(int id)
        {
            var complain = await _repository.GetByIdAsync(id);
            if (complain == null) return false;

            await _repository.DeleteAsync(complain);
            return true;
        }
    }
    }
