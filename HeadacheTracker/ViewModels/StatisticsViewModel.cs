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

        private ObservableCollection<int> _years = new ObservableCollection<int>();
        public ObservableCollection<int> Years
        {
            get => _years;
            set => SetProperty(ref _years, value);
        }

        private ObservableCollection<string> _months = new ObservableCollection<string>();
        public ObservableCollection<string> Months
        {
            get => _months;
            set => SetProperty(ref _months, value);
        }

        private string _periodLabel = string.Empty;
        public string PeriodLabel
        {
            get => _periodLabel;
            set => SetProperty(ref _periodLabel, value);
        }

        private int _selectedYear;
        public int SelectedYear
        {
            get => _selectedYear;
            set => SetProperty(ref _selectedYear, value);
        }


        private StatisticsSummary summary = new StatisticsSummary();
        public StatisticsSummary Summary
       {
            get => summary;
            set => SetProperty(ref summary, value);
        }

       
        private int selectedMonthIndex; // 0..11
        public int SelectedMonthIndex
        {
            get => selectedMonthIndex;
            set => SetProperty(ref selectedMonthIndex, value);
        }


        public StatisticsViewModel(IHeadacheRepository repository, IMedicationRepository medicationRepository, StatisticsService service)
        {
            _headacheRepository = repository;
            _medicationRepository = medicationRepository;
            _statisticsService = service;
            Summary = new StatisticsSummary();

            var now = DateTime.Now;

            // Инициализация periodLabel текущим месяцем и годом
    PeriodLabel = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(now.Month)} {now.Year}";

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
            var monthNumber = SelectedMonthIndex + 1;

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
           PeriodLabel = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(SelectedMonthIndex)} {SelectedYear}";
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
