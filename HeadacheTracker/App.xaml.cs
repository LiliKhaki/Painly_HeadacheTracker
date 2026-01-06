using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Application = Microsoft.Maui.Controls.Application;
using HeadacheTracker.Maui.Helpers;// 👈 Явное указание, какой Application использовать

namespace HeadacheTracker.Maui
{
    public partial class App : IApplication
    {
        public App(MainPage mainPage)
        {
            InitializeComponent();
            MainPage = new NavigationPage(mainPage);

        }

    }
}
