using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HeadacheTracker.Maui.ViewModels;

namespace HeadacheTracker.Maui.Views { 


    public partial class StatisticsPage : ContentPage
    {
        public StatisticsPage()
        {
            InitializeComponent();
        }
        public StatisticsPage(StatisticsViewModel viewModel): this()
        {
            
            BindingContext = viewModel;
        }
       
    }
}
