using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;

namespace HeadacheTracker.Application.UseCases;
//Обычно в этой папке хранятся отдельные “сценарии” — конкретные действия, которые может выполнять пользователь
//или система.

public class GetHeadachesByMonth
{
    private readonly IHeadacheRepository _repo;

    public GetHeadachesByMonth(IHeadacheRepository repo)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<HeadacheEntry>> ExecuteAsync(int year, int month)
    {
        // Получаем все записи за месяц напрямую из репозитория
        var entries = await _repo.GetByMonthAsync(year, month);

        // На всякий случай нормализуем даты к Date (без времени)
        return entries.Select(e => new HeadacheEntry
        {
            Id = e.Id,
            Date = e.Date.Date, // убираем время
            Intensity = e.Intensity,
            Notes = e.Notes
        });
    }
}
