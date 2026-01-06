using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;
using SQLite;
using System.Diagnostics;

namespace HeadacheTracker.Infrastructure.Repositories;

public class HeadacheRepository : IHeadacheRepository
{
    private readonly SQLiteAsyncConnection _db;
    private readonly IMedicationRepository _medicationRepository;

    public HeadacheRepository(string dbPath, IMedicationRepository medicationRepository )
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<HeadacheEntry>().Wait();
        _db.CreateTableAsync<MedicationEntry>().Wait();
        _medicationRepository = medicationRepository;
    }

    public async Task<List<HeadacheEntry>> GetByMonthAsync(int year, int month)
    {
        var all = await _db.Table<HeadacheEntry>().ToListAsync();
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

        var headaches = await _db.Table<HeadacheEntry>()
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
        await _db.InsertAsync(entry);

        System.Diagnostics.Debug.WriteLine($"[Debug] Added HeadacheEntry: Id={entry.Id}, Date={entry.Date}");

    }

    // Удалить запись
    public async Task DeleteAsync(HeadacheEntry entry)
    {
        await _medicationRepository.DeleteByHeadacheIdAsync(entry.Id);
        await _db.DeleteAsync(entry);
    }

    // Обновить запись
    public async Task UpdateAsync(HeadacheEntry entry)
    {
        await _db.UpdateAsync(entry);
    }



    // Получить все записи за определённую дату
    public async Task<IEnumerable<HeadacheEntry>> GetByDateAsync(DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return await _db.Table<HeadacheEntry>()
            .Where(x => x.Date >= start && x.Date < end)
            .ToListAsync();
    }

    
    }
