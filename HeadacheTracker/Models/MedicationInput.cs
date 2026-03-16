using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadacheTracker.Maui.Models
{
    public class MedicationInput : ObservableObject
    {
        public int Id { get; set; }
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    OnPropertyChanged(nameof(CanEnterDose));
                }
            }
        }

        private double _dose;
        public double Dose
        {
            get => _dose;
            set => SetProperty(ref _dose, value);
        }
        public bool CanEnterDose =>
 !string.IsNullOrWhiteSpace(Name);
    }

}
