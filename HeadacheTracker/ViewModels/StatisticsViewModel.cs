using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;
using HeadacheTracker.Infrastructure.Repositories;
using HeadacheTracker.Maui.Models;
using HeadacheTracker.Maui.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;


namespace HeadacheTracker.Maui.ViewModels
{
    public partial class StatisticsViewModel : ObservableObject
    {
        private readonly StatisticsService _statisticsService;
        private readonly IHeadacheRepository _headacheRepository;
        private readonly IMedicationRepository _medicationRepository;

        [ObservableProperty]
        private ObservableCollection<int> years;

        [ObservableProperty]
        private ObservableCollection<string> months;

        [ObservableProperty]
        private string periodLabel;

        [ObservableProperty]
        private int selectedYear;

        

        [ObservableProperty]
        private StatisticsSummary summary;

        [ObservableProperty]
        private int selectedMonthIndex; // 0..11

     


        public StatisticsViewModel(IHeadacheRepository repository, IMedicationRepository medicationRepository, StatisticsService service)
        {
            _headacheRepository = repository;
            _medicationRepository = medicationRepository;
            _statisticsService = service;
            Summary = new StatisticsSummary();

            var now = DateTime.Now;

            Years = new ObservableCollection<int>(
                Enumerable.Range(now.Year - 5, 6)
            );

            Months = new ObservableCollection<string>(
                CultureInfo.CurrentCulture.DateTimeFormat.MonthNames
                    .Where(m => !string.IsNullOrWhiteSpace(m))
                    .ToList()
            );

            SelectedYear = now.Year;
            selectedMonthIndex = now.Month - 1; // 🔴 ВАЖНО: после Months

            Debug.WriteLine($"Months count: {Months.Count}");
            Debug.WriteLine($"SelectedMonthIndex: {SelectedMonthIndex}");

        }

        public async Task LoadStatisticsAsync()
        {
            var monthNumber = selectedMonthIndex + 1;

            var periodStart = new DateTime(SelectedYear, monthNumber, 1);
            var periodEnd = periodStart.AddMonths(1).AddDays(-1);

            Debug.WriteLine($"Year={SelectedYear}, Month={monthNumber}");


            var allHeadaches = await _headacheRepository.GetAllAsync() ?? new List<HeadacheEntry>();
            Debug.WriteLine($"All headaches: {allHeadaches.Count}");
            var medications = await _medicationRepository.GetAllAsync();
            Debug.WriteLine($"All medications: {medications.Count}");

            _statisticsService.CalculateSummary(
     Summary,
     allHeadaches, medications,
     periodStart,
     periodEnd);


            // Aktualisieren der Periodenbeschriftung
            periodLabel = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonthIndex)} {SelectedYear}";
        }

        [RelayCommand]
        private async Task RefreshStatisticsAsync()
        {

            try
            {
                Debug.WriteLine("RefreshStatisticsAsync called");
                await LoadStatisticsAsync();
                Debug.WriteLine("=== RefreshStatisticsCommand EXECUTED ===");

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex + " из RefreshStatisticsAsync выброшен");

                
            }
        }

        

    }

}
