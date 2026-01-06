using Microsoft.Maui.Controls;

using HeadacheTracker.Maui.ViewModels;

namespace HeadacheTracker.Maui
{
    public partial class MainPage : ContentPage
    {

        public MainPage(CalendarViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

            System.Diagnostics.Debug.WriteLine($"DB PATH: {FileSystem.AppDataDirectory}");
            


        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (BindingContext is CalendarViewModel vm)
                await vm.LoadMonthAsync();
        }



    }
}
