using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HeadacheTracker.Application.UseCases;
using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;
using HeadacheTracker.Infrastructure.Repositories;
using HeadacheTracker.Maui.Messages;
using HeadacheTracker.Maui.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Formats.Tar;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HeadacheTracker.Maui.ViewModels
{
    public class CalendarViewModel : ObservableObject, INotifyPropertyChanged
    {
        #region Fields

        #region Dependencies
        private readonly IHeadacheRepository _repository;
        private readonly IMedicationRepository _medicationRepository;
        private readonly GetHeadachesByMonth _getHeadaches;
        private readonly IServiceProvider _services;
        #endregion

        #region Calendar State
        public int Year { get; private set; }
        public int Month { get; private set; }
        public string CurrentMonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");

        public ObservableCollection<CalendarDay> Days { get; } = new();
        #endregion

        #region Selected Day
        private CalendarDay? _selectedDay;
        public CalendarDay? SelectedDay
        {
            get => _selectedDay;
            set
            {
                if (SetProperty(ref _selectedDay, value))
                {
                    foreach (var day in Days)
                        day.IsSelected = day.Date.Date == value?.Date.Date;

                    _ = LoadEntriesForSelectedDayAsync();
                }
            }
        }
        #endregion

        #region Entries for Selected Day (UI)
        private ObservableCollection<HeadacheEntryViewModel> _headacheEntriesForSelectedDay = new();
        public ObservableCollection<HeadacheEntryViewModel> HeadacheEntriesForSelectedDay
        {
            get => _headacheEntriesForSelectedDay;
            set => SetProperty(ref _headacheEntriesForSelectedDay, value);
        }
        #endregion

        #region Selected Entry
        private HeadacheEntryViewModel? _selectedHeadacheEntry;
        public HeadacheEntryViewModel? SelectedHeadacheEntry
        {
            get => _selectedHeadacheEntry;
            set
            {
                if (SetProperty(ref _selectedHeadacheEntry, value))
                {
                    foreach (var it in HeadacheEntriesForSelectedDay)
                        it.IsSelected = false;

                    if (value != null)
                        value.IsSelected = true;

                    EditRecordCommand.NotifyCanExecuteChanged();
                    DeleteRecordCommand.NotifyCanExecuteChanged();
                }
            }
        }
        #endregion

        #region Commands
        public IRelayCommand AddRecordCommand { get; }
        public IRelayCommand EditRecordCommand { get; }
        public IRelayCommand DeleteRecordCommand { get; }

        public ICommand NextMonthCommand { get; }
        public ICommand PreviousMonthCommand { get; }
        public ICommand OpenStatisticsCommand { get; }
        public ICommand OpenAddEntryCommand { get; }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        private bool CanDeleteRecord()
        {
            return SelectedHeadacheEntry != null;
        }

        private bool CanEdit()
        {
            return SelectedHeadacheEntry != null;
        }


        #endregion

        public CalendarViewModel(
    IHeadacheRepository repository,
    IMedicationRepository medicationRepository,
    IServiceProvider services)
        {
            Debug.WriteLine("VM constructor START");

            _repository = repository;
            _medicationRepository = medicationRepository;
            _services = services;

            _getHeadaches = new GetHeadachesByMonth(_repository);

            Days = new ObservableCollection<CalendarDay>();

            OpenStatisticsCommand = new Command(async () => await OnOpenStatistics());
            AddRecordCommand = new RelayCommand(async () => await AddRecordAsync());
            OpenAddEntryCommand = new RelayCommand(async () => await OpenAddEntryPageAsync());
            EditRecordCommand = new RelayCommand(OnEditRecord, CanEdit);
            DeleteRecordCommand = new RelayCommand(OnDeleteRecord, CanDeleteRecord);

            NextMonthCommand = new RelayCommand(async () => await ChangeMonthAsync(1));
            PreviousMonthCommand = new RelayCommand(async () => await ChangeMonthAsync(-1));

            var today = DateTime.Today;
            Year = today.Year;
            Month = today.Month;

            WeakReferenceMessenger.Default.Register<EntryAddedMessage>(this, (_, message) =>
            {
                var day = Days.FirstOrDefault(d => d.Date.Date == message.Value.Date);
                if (day != null)
                    SelectedDay = day;
            });
            Debug.WriteLine("VM constructor END");

        }

        public async Task InitializeAsync()
        {
            Debug.WriteLine("InitializeAsync started");

            await LoadMonthAsync();
            await LoadEntriesForSelectedDayAsync();

            Debug.WriteLine($"Days count after init: {Days.Count}");
        }


        private async Task LoadEntriesForSelectedDayAsync()
        {
            try
            {
                var date = (SelectedDay?.Date ?? DateTime.Today).Date;

                // 1. Получаем записи за этот день
                var headacheEntries = await _repository.GetByDateAsync(date);

                var combinedList = new List<HeadacheEntryViewModel>();

                foreach (var headache in headacheEntries)
                {
                    // 2. Получаем связанные лекарства
                    var meds = await _medicationRepository.GetByHeadacheIdAsync(headache.Id);

                    // 3. Создаем VM
                    var vm = new HeadacheEntryViewModel
                    {
                        Id = headache.Id,
                        Date = headache.Date,
                        Intensity = headache.Intensity,
                        Notes = headache.Notes
                    };

                    // 4. Добавляем лекарства
                    foreach (var m in meds)
                    {
                        vm.Medications.Add(new MedicationEntry
                        {
                            Id = m.Id,
                            HeadacheEntryId = m.HeadacheEntryId,
                            Medication = m.Medication,
                            Dose = m.Dose
                        });
                    }

                    combinedList.Add(vm);
                }

                // 5. Обновляем 
                HeadacheEntriesForSelectedDay =
                    new ObservableCollection<HeadacheEntryViewModel>(combinedList);

                OnPropertyChanged(nameof(HeadacheEntriesForSelectedDay));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[LoadEntriesForSelectedDayAsync] Ошибка: {ex}");
            }
        }

        private async Task ChangeMonthAsync(int delta)
        {
            Month += delta;
            if (Month > 12)
            {
                Month = 1;
                Year++;
            }
            else if (Month < 1)
            {
                Month = 12;
                Year--;
            }

            await LoadMonthAsync();
            OnPropertyChanged(nameof(CurrentMonthName));
        }

        public async Task LoadMonthAsync()
        {
            Debug.WriteLine("LoadMonthAsync START");

            var entries = new List<HeadacheEntry>();

            if (_getHeadaches != null)
            {
                var result = await _getHeadaches.ExecuteAsync(Year, Month);
                entries = result?.ToList() ?? new List<HeadacheEntry>();
            }

            var entryDates = entries
                .Select(e => e.Date.Date)
                .Select(d => DateTime.SpecifyKind(d, DateTimeKind.Unspecified));

            var days = GenerateDays(Year, Month, entryDates);

            Days.Clear();
            foreach (var d in days)
                Days.Add(d);

            Debug.WriteLine($"Days count after load: {Days.Count}");
            Debug.WriteLine("LoadMonthAsync КОНЕЦ");
        }
      
        void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // GenerateDays / LoadMonthAsync в ViewModel
        private List<CalendarDay> GenerateDays(
    int year,
    int month,
    IEnumerable<DateTime> entryDates)
        {
            var result = new List<CalendarDay>();

            var firstDay = new DateTime(year, month, 1);
            int daysInMonth = DateTime.DaysInMonth(year, month);

            var datesWithEntries = entryDates?
                .Select(d => d.Date)
                .ToHashSet()
                ?? new HashSet<DateTime>();

            int startOffset = ((int)firstDay.DayOfWeek + 6) % 7;

            // предыдущий месяц
            var prevMonth = firstDay.AddMonths(-1);
            int daysInPrevMonth = DateTime.DaysInMonth(prevMonth.Year, prevMonth.Month);

            for (int i = daysInPrevMonth - startOffset + 1; i <= daysInPrevMonth; i++)
            {
                var date = new DateTime(prevMonth.Year, prevMonth.Month, i);
                result.Add(new CalendarDay(date)
                {
                    IsFromCurrentMonth = false,
                    HasEntry = datesWithEntries.Contains(date.Date)
                });
            }

            // текущий месяц
            for (int i = 1; i <= daysInMonth; i++)
            {
                var date = new DateTime(year, month, i);
                result.Add(new CalendarDay(date)
                {
                    IsFromCurrentMonth = true,
                    HasEntry = datesWithEntries.Contains(date.Date)
                });
            }

            // следующий месяц
            int remaining = 42 - result.Count;
            var nextMonth = firstDay.AddMonths(1);

            for (int i = 1; i <= remaining; i++)
            {
                var date = new DateTime(nextMonth.Year, nextMonth.Month, i);
                result.Add(new CalendarDay(date)
                {
                    IsFromCurrentMonth = false,
                    HasEntry = datesWithEntries.Contains(date.Date)
                });
            }

            // today
            var today = result.FirstOrDefault(d => d.Date.Date == DateTime.Today.Date);
            if (today != null)
                today.IsToday = true;

            return result;
        }
        private async Task AddRecordAsync()
        {
            var entryDate = SelectedDay?.Date ?? DateTime.Today;

            var viewModel = new AddEntryViewModel(_repository, _medicationRepository, entryDate);

            var popup = new AddEntryPopup(viewModel)
            {
                BindingContext = viewModel
            };

            await Microsoft.Maui.Controls.Application.Current.MainPage.ShowPopupAsync(popup);
        }

        private async Task OpenAddEntryPageAsync()
        {
            if (SelectedDay == null)
                SelectedDay = new CalendarDay(DateTime.Today);

            // создаём ViewModel для popup
            var viewModel = new AddEntryViewModel(
                _repository,
                _medicationRepository,
                SelectedDay.Date
            );

            // создаём popup и передаём ViewModel в конструктор
            var popup = new AddEntryPopup(viewModel);

            // показываем popup поверх текущей страницы
            await Microsoft.Maui.Controls.Application.Current.MainPage.ShowPopupAsync(popup);
        }

        private async void OnEditRecord()
        {
            var entryVm = SelectedHeadacheEntry;
            if (entryVm == null)
                return;

            Debug.WriteLine($"[CalendarViewModel] Editing record Id={entryVm.Id}");

            var vm = new AddEntryViewModel(_repository, _medicationRepository, entryVm.Date)
            {
                IsEditMode = true,
                ExistingEntryId = entryVm.Id
            };

            // Загружаем головную боль и все связанные медикаменты
            var headacheEntry = entryVm.ToHeadacheEntry();
            var meds = await _medicationRepository.GetByHeadacheIdAsync(entryVm.Id);
            vm.LoadFromExisting(headacheEntry, new ObservableCollection<MedicationEntry>(meds));

            // создаём popup
            var popup = new AddEntryPopup(vm);
            await Microsoft.Maui.Controls.Application.Current.MainPage.ShowPopupAsync(popup);

            // перезагружаем список после редактирования
            await LoadEntriesForSelectedDayAsync();
            await LoadMonthAsync();
        }

        private async void OnDeleteRecord()
        {
            if (SelectedHeadacheEntry == null)
                return;

            // Подтверждение удаления (можно отключить, если мешает)
            bool confirm = await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                "Delete Entry",
                "Are you sure you want to delete this record?",
                "Yes",
                "No");

            if (!confirm)
                return;

            try
            {
                int headacheId = SelectedHeadacheEntry.Id;

                // 1️⃣ Удаление медикаментов
                var meds = await _medicationRepository.GetByHeadacheIdAsync(headacheId);
                foreach (var med in meds)
                {
                    await _medicationRepository.DeleteAsync(med.Id);
                }

                // 2️⃣ Удаление записи головной боли
                await _repository.DeleteAsync(new HeadacheEntry
                {
                    Id = SelectedHeadacheEntry.Id,
                    Date = SelectedHeadacheEntry.Date,
                    Intensity = SelectedHeadacheEntry.Intensity,
                    Notes = SelectedHeadacheEntry.Notes
                });


                // 3️⃣ Удаляем из списка UI
                HeadacheEntriesForSelectedDay.Remove(SelectedHeadacheEntry);

                // 4️⃣ Сброс выделения
                SelectedHeadacheEntry = null;

                // 5️⃣ Обновляем команды
                EditRecordCommand.NotifyCanExecuteChanged();
                DeleteRecordCommand.NotifyCanExecuteChanged();

                // 6️⃣ Уведомляем календарь, если он что-то подкрашивает
                WeakReferenceMessenger.Default.Send(new EntryAddedMessage(_selectedDay.Date));
            }
            catch (Exception ex)
            {
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"Failed to delete the record: {ex.Message}",
                    "OK");
            }
        }

        private async Task OnOpenStatistics()
        {
            var vm = _services.GetRequiredService<StatisticsViewModel>();
            var page = new StatisticsPage(vm);

            await Microsoft.Maui.Controls.Application.Current.MainPage.Navigation.PushAsync(page);
        }

        
    }
}
