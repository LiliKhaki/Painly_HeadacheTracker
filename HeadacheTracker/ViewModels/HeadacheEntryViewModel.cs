using CommunityToolkit.Mvvm.ComponentModel;
using HeadacheTracker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadacheTracker.Maui.ViewModels
{
    //ViewModel-класс, отвечающий за редактирование/создание одной записи, и используется внутри
    //попапа AddEntryPopup

    public class HeadacheEntryViewModel : ObservableObject
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Intensity { get; set; }  
        public string? Notes { get; set; }
        public string? MedicationName { get; set; }
        public double? Dose { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public ObservableCollection<MedicationEntry> Medications { get; set; }
    = new ObservableCollection<MedicationEntry>();


        public HeadacheEntry ToHeadacheEntry()
        {
            return new HeadacheEntry
            {
                Id = this.Id,
                Date = this.Date,
                Intensity = this.Intensity,
                Notes = this.Notes
            };
        }

  

    }

}
