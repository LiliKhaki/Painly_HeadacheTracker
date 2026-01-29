using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;

namespace HeadacheTracker.Infrastructure.Repositories
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public MedicationRepository(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<MedicationEntry>().Wait();
        }

        public async Task AddAsync(MedicationEntry entry)
        {
            await _database.InsertAsync(entry);
            
            System.Diagnostics.Debug.WriteLine($"[Debug] Added MedicationEntry: HeadacheId={entry.HeadacheEntryId}, Medication={entry.Medication}");

        }

        public async Task<IEnumerable<MedicationEntry>> GetByHeadacheIdAsync(int headacheId)
        {
            return await _database.Table<MedicationEntry>()
                                  .Where(m => m.HeadacheEntryId == headacheId)
                                  .ToListAsync();
        }


        public async Task<List<MedicationEntry>> GetAllAsync()
        {
            return await _database
                .Table<MedicationEntry>()
                .ToListAsync();
        }



        public async Task UpdateAsync(MedicationEntry entry)
        {
            await _database.UpdateAsync(entry);
        }

        public async Task DeleteAsync(int id)
        {
            await _database.DeleteAsync<MedicationEntry>(id);
        }

        public async Task DeleteByHeadacheIdAsync(int headacheId)
        {
            var medications = await _database.Table<MedicationEntry>()
                .Where(m => m.HeadacheEntryId == headacheId)
                .ToListAsync();

            foreach (var med in medications)
            {
                await _database.DeleteAsync(med);
            }
        }



    }
}
