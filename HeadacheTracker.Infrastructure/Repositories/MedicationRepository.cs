using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;
using SQLite;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HeadacheTracker.Infrastructure.Repositories
{
    public class MedicationRepository : IMedicationRepository
    {
        private readonly SQLiteAsyncConnection _database;

        public MedicationRepository(SQLiteAsyncConnection database)
        {
            _database = database;
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

            await _database.ExecuteAsync(
    "DELETE FROM MedicationEntry WHERE HeadacheEntryId = ?",
    headacheId);

        }

        public async Task<List<MedicationEntry>> GetByDateAsync(DateTime date)
        {
            try
            {
                var result = await _database.QueryAsync<MedicationEntry>(
                    @"
            SELECT m.Id,
                   m.HeadacheEntryId,
                   m.Medication,
                   m.Dose
            FROM MedicationEntry m
            INNER JOIN HeadacheEntry h
                ON m.HeadacheEntryId = h.Id
            WHERE date(h.Date) = date(?)
            ",
                    date.Date);

                return result;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[MedicationRepository.GetByDateAsync] Ошибка: {ex}");
                return new List<MedicationEntry>();
            }
        }

    }
}
