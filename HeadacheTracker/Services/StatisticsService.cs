using HeadacheTracker.Domain.Entities;
using HeadacheTracker.Maui.Models;
using System.Diagnostics;

namespace HeadacheTracker.Maui.Services
{   
        public class StatisticsService
        {
        //Взять уже загруженные эпизоды боли и посчитать пользовательскую статистику
        //за ЧЁТКО ЗАДАННЫЙ период времени.
        public void CalculateSummary(
      StatisticsSummary summary,
      IEnumerable<HeadacheEntry> headaches, IEnumerable<MedicationEntry> medications,
      DateTime periodStart,
      DateTime periodEnd)
        {
            try
            {
                Debug.WriteLine("=== CalculateSummary CALLED ===");

                var filtered = headaches
                    .Where(h => h.Date.Date >= periodStart.Date &&
                                h.Date.Date <= periodEnd.Date)
                    .ToList();

                Debug.WriteLine($"Filtered headaches count: {filtered.Count}");

                var daysWithPain = filtered
                    .Select(h => h.Date.Date)
                    .Distinct()
                    .ToHashSet();

                Debug.WriteLine($"DaysWithPain calculated: {daysWithPain.Count}");

                summary.DaysWithPain = daysWithPain.Count;
                summary.PainEpisodes = filtered.Count;
                summary.AverageIntensity = filtered.Any()
                    ? filtered.Average(h => h.Intensity)
                    : 0;

                Debug.WriteLine("Basic fields set");

                var allDays = new List<DateTime>();
                for (var day = periodStart.Date; day <= periodEnd.Date; day = day.AddDays(1))
                    allDays.Add(day);

                Debug.WriteLine($"AllDays count: {allDays.Count}");

                summary.PainFreeDays = allDays.Count(d => !daysWithPain.Contains(d));
                summary.LongestPainFreeStreak =
                    CalculateLongestStreak(allDays, daysWithPain);

                summary.DaysWithMedication = filtered
                    .Where(h => h.Medications?.Any() == true)
                    .Select(h => h.Date.Date)
                    .Distinct()
                    .Count();

                // id болей за период
                var headacheIdsInPeriod = filtered
                    .Select(h => h.Id)
                    .ToHashSet();

                // дни, когда при боли принимались медикаменты
                var daysWithMedication = medications
    .Where(m => headacheIdsInPeriod.Contains(m.HeadacheEntryId))
    .Select(m =>
        headaches
            .First(h => h.Id == m.HeadacheEntryId)
            .Date.Date)
    .Distinct()
    .Count();
                summary.DaysWithMedication = daysWithMedication;


                Debug.WriteLine("=== CalculateSummary FINISHED ===");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("!!! EXCEPTION IN CalculateSummary !!!");
                Debug.WriteLine(ex.ToString());
                throw;
            }
        }


        private int CalculateLongestStreak(
IEnumerable<DateTime> allDays,
HashSet<DateTime> daysWithPain)

        {
            int longest = 0;
                int current = 0;

                foreach (var day in allDays)
                {
                    if (!daysWithPain.Contains(day))
                    {
                        current++;
                        if (current > longest)
                            longest = current;
                    }
                    else
                    {
                        current = 0;
                    }
                }

                return longest;
            }
        }
    }
