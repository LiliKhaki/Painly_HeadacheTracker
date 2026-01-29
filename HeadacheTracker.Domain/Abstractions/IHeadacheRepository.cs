using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HeadacheTracker.Domain.Entities;

namespace HeadacheTracker.Domain.Abstractions;

public interface IHeadacheRepository
//“Я умею возвращать, сохранять и искать записи о головных болях”.
{
    Task<List<HeadacheEntry>> GetAllAsync();
    //Task<List<HeadacheEntry>> GetByDateAsync(DateTime date);
    Task<IEnumerable<HeadacheEntry>> GetByDateAsync(DateTime date);
    Task<List<HeadacheEntry>> GetByMonthAsync(int year, int month);

    Task AddAsync(HeadacheEntry entry);
    Task DeleteAsync(HeadacheEntry entry);
    Task UpdateAsync(HeadacheEntry entry);
    
}
