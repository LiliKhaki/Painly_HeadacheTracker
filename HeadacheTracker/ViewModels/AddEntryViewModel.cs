using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;
using HeadacheTracker.Maui.Messages;
using HeadacheTracker.Maui.Models;
using HeadacheTracker.Maui.Resources.Strings;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;



namespace HeadacheTracker.Maui.ViewModels
{
    public partial class AddEntryViewModel : ObservableObject, INotifyPropertyChanged
    {
        private readonly IHeadacheRepository _headacheRepository;
        private readonly IMedicationRepository _medicationRepository;

        public int ExistingEntryId { get;  set; }
        public bool IsEditMode { get;  set; } = false;
        public string Title { get; }


        private int? _intensity;
        public int? Intensity
        {
            get => _intensity;
            set => SetProperty(ref _intensity, value);
           
        }

        public ObservableCollection<int> Intensities { get; } = new ObservableCollection<int>(Enumerable.Range(1, 10));
        private string? _notes;
        public string? Notes
        {
            get => _notes;
            set => SetProperty(ref _notes, value);
        }
        public DateTime EntryDate { get; set; }

        //public event PropertyChangedEventHandler? PropertyChanged;

        //protected void OnPropertyChanged([CallerMemberName] string name = null)
        //    => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // Коллекция для динамических медикаментов
        public ObservableCollection<MedicationInput> Medications { get; } = new ObservableCollection<MedicationInput>();

        public IRelayCommand AddMedicationCommand { get; }
        public IRelayCommand<MedicationInput> RemoveMedicationCommand { get; }
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public ICommand FocusNextCommand { get; }

        public event Action? RequestClose;

        public AddEntryViewModel(IHeadacheRepository headacheRepo, IMedicationRepository medicationRepo, DateTime selectedDate, bool isEditMode)
        {
            _headacheRepository = headacheRepo;
            _medicationRepository = medicationRepo;
            EntryDate = selectedDate;
            Title = isEditMode
        ? AppResources.EditHeadacheRecord
        : AppResources.AddRecord;

            AddMedicationCommand = new RelayCommand(() =>
            {
                Medications.Add(new MedicationInput());
            });

            RemoveMedicationCommand = new RelayCommand<MedicationInput>(med =>
            {
                if (med != null)
                    Medications.Remove(med);
            });

            SaveCommand = new RelayCommand(
                async () => await SaveAsync()
            );
            CancelCommand = new RelayCommand(OnCancel);
            FocusNextCommand = new Command<Entry>(entry =>
            {
                entry?.Focus(); // простейший способ
            });
        }

        public void LoadFromExisting(HeadacheEntry headache, ObservableCollection<MedicationEntry> meds)
        {
            IsEditMode = true;
            ExistingEntryId = headache.Id;

            Intensity = headache.Intensity;
            Notes = headache.Notes;
            EntryDate = headache.Date;

            Medications.Clear();
            foreach (var med in meds)
            {
                Medications.Add(new MedicationInput
                {
                    Id = med.Id,
                    Name = med.Medication ?? string.Empty,
                    Dose = med.Dose ?? 0.0
                });
            }
        }

        private async Task SaveAsync()
        {
            if (Intensity == null)
            {
                var page = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page;
                if (page != null)
                    await page.DisplayAlert(
            AppResources.Error,   // заголовок окна
            AppResources.PickUpIntensity, // сообщение
            AppResources.OkButton                  // кнопка подтверждения
        );
                return;
            }

            try
            {
                // 1️⃣ Сохраняем или обновляем головную боль
                var headache = new HeadacheEntry
                {
                    Id = ExistingEntryId,
                    Date = EntryDate,
                    Intensity = (int)Intensity,
                    Notes = string.IsNullOrWhiteSpace(Notes) ? null : Notes
                };

                if (IsEditMode)
                {
                    await _headacheRepository.UpdateAsync(headache);
                }
                else
                {
                    await _headacheRepository.AddAsync(headache);
                    ExistingEntryId = headache.Id; // получаем ID для медикаментов
                }

                // 2️⃣ Сохраняем медикаменты
                var existingMeds = await _medicationRepository.GetByHeadacheIdAsync(headache.Id);

                // Удаляем те, которых нет в Medications
                foreach (var oldMed in existingMeds)
                {
                    if (!Medications.Any(m => m.Id == oldMed.Id))
                        await _medicationRepository.DeleteAsync(oldMed.Id);
                }

                // Добавляем новые или обновляем существующие
                foreach (var med in Medications)
                {
                    var entry = new MedicationEntry
                    {
                        Id = med.Id,
                        HeadacheEntryId = headache.Id,
                        Medication = med.Name,
                        Dose = med.Dose
                    };

                    if (entry.Id == 0)
                        await _medicationRepository.AddAsync(entry);
                    else
                        await _medicationRepository.UpdateAsync(entry);
                }

                // 3️⃣ Уведомляем и закрываем
                WeakReferenceMessenger.Default.Send(new EntryAddedMessage(headache.Date));
                RequestClose?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[SaveEntry] ERROR: {ex}");

                var page = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page;
                if (page != null)
                {
                    await page.DisplayAlert(
                        AppResources.SaveErrorTitle,
                        AppResources.SaveErrorMessage,
                        AppResources.OkButton);
                }
                else
                {
                    Debug.WriteLine("[AddEntryViewModel] Unable to show alert: no active Page found.");
                }
            }
        }

        private void OnCancel()
        {
            Debug.WriteLine("[AddEntryViewModel] Cancel invoked");
            RequestClose?.Invoke();
        }



    }
}
