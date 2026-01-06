using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;
using HeadacheTracker.Maui.Messages;
using HeadacheTracker.Maui.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace HeadacheTracker.Maui.ViewModels
{
    public partial class AddEntryViewModel : ObservableObject
    {
        private readonly IHeadacheRepository _headacheRepository;
        private readonly IMedicationRepository _medicationRepository;

        public int ExistingEntryId { get;  set; }
        public bool IsEditMode { get;  set; } = false;

        public int Intensity { get; set; }
        public string? Notes { get; set; }
        public DateTime EntryDate { get; set; }

        // Коллекция для динамических медикаментов
        public ObservableCollection<MedicationInput> Medications { get; } = new ObservableCollection<MedicationInput>();

        public IRelayCommand AddMedicationCommand { get; }
        public IRelayCommand<MedicationInput> RemoveMedicationCommand { get; }
        public IRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }

        public event Action? RequestClose;

        public AddEntryViewModel(IHeadacheRepository headacheRepo, IMedicationRepository medicationRepo, DateTime selectedDate)
        {
            _headacheRepository = headacheRepo;
            _medicationRepository = medicationRepo;
            EntryDate = selectedDate;

            AddMedicationCommand = new RelayCommand(() =>
            {
                Medications.Add(new MedicationInput());
            });

            RemoveMedicationCommand = new RelayCommand<MedicationInput>(med =>
            {
                if (med != null)
                    Medications.Remove(med);
            });

            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new RelayCommand(OnCancel);
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
                    Name = med.Medication,
                    Dose = (double)med.Dose
                });
            }
        }

        private async Task SaveAsync()
        {
            try
            {
                // 1️⃣ Сохраняем или обновляем головную боль
                var headache = new HeadacheEntry
                {
                    Id = ExistingEntryId,
                    Date = EntryDate,
                    Intensity = Intensity,
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
                Debug.WriteLine($"[SaveAsync] Ошибка: {ex}");
                await Microsoft.Maui.Controls.Application.Current.MainPage.DisplayAlert(
                    "Ошибка",
                    $"Не удалось сохранить запись: {ex.Message}",
                    "OK");
            }
        }

        private void OnCancel()
        {
            Debug.WriteLine("[AddEntryViewModel] Cancel invoked");
            RequestClose?.Invoke();
        }
    }
}
