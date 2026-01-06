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
        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private double _dose;
        public double Dose
        {
            get => _dose;
            set => SetProperty(ref _dose, value);
        }
    }

}
