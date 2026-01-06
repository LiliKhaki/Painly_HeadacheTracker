using CommunityToolkit.Mvvm.ComponentModel;
using HeadacheTracker.Domain.Entities;
using HeadacheTracker.Maui.Models;
using HeadacheTracker.Maui.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;


namespace HeadacheTracker.Maui.ViewModels
{
    public partial class StatisticsViewModel : ObservableObject
    {
        private readonly StatisticsService _statisticsService;

        public StatisticsViewModel(StatisticsService service)
        {
            _statisticsService = service;
        }

        // Период для статистики
        [ObservableProperty]
        private int selectedYear;

        [ObservableProperty]
        private int selectedMonth; // 1..12, 0 значит "весь год"

        // Результат статистики
        [ObservableProperty]
        private StatisticsSummary summary;

        /// <summary>
        /// Вычислить статистику за выбранный период
        /// </summary>
        public void LoadStatistics(ObservableCollection<HeadacheEntry> allHeadaches)
        {
            if (allHeadaches == null || !allHeadaches.Any())
            {
                Summary = new StatisticsSummary();
                return;
            }

            // Фильтруем по выбранному периоду
            var filteredHeadaches = allHeadaches.AsEnumerable();

            if (SelectedMonth >= 1 && SelectedMonth <= 12)
            {
                filteredHeadaches = filteredHeadaches
                    .Where(h => h.Date.Year == SelectedYear && h.Date.Month == SelectedMonth);
            }
            else
            {
                filteredHeadaches = filteredHeadaches
                    .Where(h => h.Date.Year == SelectedYear);
            }

            // Вычисляем статистику через сервис
            Summary = _statisticsService.CalculateSummary(filteredHeadaches);
        }
    }
}
