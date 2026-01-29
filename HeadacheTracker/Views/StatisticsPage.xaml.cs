using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Maui.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is StatisticsViewModel vm)
            {
                // временно: передаём уже загруженные данные
                _ = vm.LoadStatisticsAsync();
            }
        }
    }
}
