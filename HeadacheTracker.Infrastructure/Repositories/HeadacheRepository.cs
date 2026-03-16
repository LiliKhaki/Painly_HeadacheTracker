using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;
using SQLite;
using System.Diagnostics;

namespace HeadacheTracker.Infrastructure.Repositories;

public class HeadacheRepository : IHeadacheRepository
{
    private readonly SQLiteAsyncConnection _database;
    private readonly IMedicationRepository _medicationRepository;

    public HeadacheRepository(SQLiteAsyncConnection database, IMedicationRepository medicationRepository )
    {
        _database = database;
        _medicationRepository = medicationRepository;
    }

    public async Task<List<HeadacheEntry>> GetByMonthAsync(int year, int month)
    {
        var all = await _database.Table<HeadacheEntry>().ToListAsync();
        Debug.WriteLine("=== ALL HEADACHE ENTRIES IN DB ===");
        foreach (var e in all)
        {
            Debug.WriteLine($"Id={e.Id}, Date={e.Date:yyyy-MM-dd HH:mm:ss}, Kind={e.Date.Kind}");
        }
        Debug.WriteLine("=== END ===");

        var firstDay = new DateTime(year, month, 1);
        var firstDayNextMonth = firstDay.AddMonths(1);


        foreach (var e in all)
        {
            Debug.WriteLine($"[DB] Id={e.Id}, Date={e.Date:O}, Kind={e.Date.Kind}, Intensity={e.Intensity}");
        }

        var headaches = await _database.Table<HeadacheEntry>()
       .Where(e => e.Date >= firstDay && e.Date < firstDayNextMonth)
       .ToListAsync();

        foreach (var headache in headaches)
        {
            headache.Medications =
                (List<MedicationEntry>)await _medicationRepository.GetByHeadacheIdAsync(headache.Id);
        }

        return headaches;
    }

    // Добавить новую запись
    public async Task AddAsync(HeadacheEntry entry)
    {
        await _database.InsertAsync(entry);

        System.Diagnostics.Debug.WriteLine($"[Debug] Added HeadacheEntry: Id={entry.Id}, Date={entry.Date}");

    }

    // Удалить запись
    public async Task DeleteAsync(HeadacheEntry entry)
    {
        await _medicationRepository.DeleteByHeadacheIdAsync(entry.Id);
        await _database.DeleteAsync(entry);
    }

    // Обновить запись
    public async Task UpdateAsync(HeadacheEntry entry)
    {
        await _database.UpdateAsync(entry);
    }



    // Получить все записи за определённую дату
    public async Task<IEnumerable<HeadacheEntry>> GetByDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return await _database.Table<HeadacheEntry>()
            .Where(x => x.Date >= start && x.Date < end)
            .ToListAsync();
    }

    public async Task<List<HeadacheEntry>> GetAllAsync()
    {
        return await _database.Table<HeadacheEntry>().ToListAsync();
    }

    public async Task<List<(HeadacheEntry Headache, MedicationEntry? Medication)>>
    GetByDateWithMedicationsAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        var result = await _database.QueryAsync<HeadacheMedicationDto>(
            @"
        SELECT 
            h.Id as HeadacheId,
            h.Date,
            h.Intensity,
            h.Notes,

            m.Id as MedicationId,
            m.HeadacheEntryId,
            m.Medication,
            m.Dose

        FROM HeadacheEntry h
        LEFT JOIN MedicationEntry m
            ON h.Id = m.HeadacheEntryId

        WHERE h.Date >= ? AND h.Date < ?
        ",
            start, end);

        return result.Select(r => r.ToTuple()).ToList();
    }


}
