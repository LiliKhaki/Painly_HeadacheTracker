using HeadacheTracker.Maui.Helpers;// 👈 Явное указание, какой Application использовать
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Application = Microsoft.Maui.Controls.Application;
using HeadacheTracker.Infrastructure;

namespace HeadacheTracker.Maui
{
    public partial class App : IApplication
    {
        private readonly DatabaseInitializer _initializer;
        private readonly MainPage _mainPage;
       

        public App(DatabaseInitializer initializer, MainPage mainPage)
        {
            InitializeComponent();
            _initializer = initializer;
            _mainPage = mainPage;

            UserAppTheme = AppTheme.Dark;

            Task.Run(async () => await _initializer.InitializeAsync());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new NavigationPage(_mainPage));
        }
    }
}
