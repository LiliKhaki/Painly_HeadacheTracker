using HeadacheTracker.Domain.Entities;

namespace HeadacheTracker.Domain.Abstractions
{
    public interface IMedicationRepository
    {
        Task<List<MedicationEntry>> GetAllAsync();
        Task AddAsync(MedicationEntry entry);
        Task<IEnumerable<MedicationEntry>> GetByHeadacheIdAsync(int headacheId);
        
        Task DeleteAsync(int id);
        Task DeleteByHeadacheIdAsync(int headacheId);
        
        Task UpdateAsync(MedicationEntry entry);
    }
}
