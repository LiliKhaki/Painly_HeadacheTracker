using HeadacheTracker.Domain.Entities;
using HeadacheTracker.Maui.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using HeadacheTracker.Maui.ViewModels;

namespace HeadacheTracker.Maui.Services
{   
        public class StatisticsService
        {
            public StatisticsSummary CalculateSummary(
                IEnumerable<HeadacheEntry> headaches)
            {
                var summary = new StatisticsSummary();

                // Список уникальных дней с болью
                var daysWithPain = headaches
                    .Select(h => h.Date.Date)
                    .Distinct()
                    .ToList();
                summary.DaysWithPain = daysWithPain.Count;

                // Общее количество эпизодов боли
                summary.PainEpisodes = headaches.Count();

                // Средняя интенсивность
                summary.AverageIntensity = headaches.Any()
                    ? headaches.Average(h => h.Intensity)
                    : 0;

                // Определяем весь период по данным
                List<DateTime> allDays;
                if (headaches.Any())
                {
                    var start = headaches.Min(h => h.Date.Date);
                    var end = headaches.Max(h => h.Date.Date);
                    allDays = new List<DateTime>();
                    for (var day = start; day <= end; day = day.AddDays(1))
                    {
                        allDays.Add(day);
                    }
                }
                else
                {
                    allDays = new List<DateTime>();
                }

                // Количество дней без боли
                summary.PainFreeDays = allDays.Count(d => !daysWithPain.Contains(d));

                // Самый длинный период без боли
                summary.LongestPainFreeStreak = CalculateLongestStreak(allDays, daysWithPain);

                // Дни с приёмом медикаментов
                summary.DaysWithMedication = headaches
                    .Where(h => h.Medications != null && h.Medications.Any())
                    .Select(h => h.Date.Date)
                    .Distinct()
                    .Count();

                // Средняя доза пока оставляем null
                summary.AverageDose = null;

                return summary;
            }

            private int CalculateLongestStreak(List<DateTime> allDays, List<DateTime> daysWithPain)
            {
                int longest = 0;
                int current = 0;

                foreach (var day in allDays.OrderBy(d => d))
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
