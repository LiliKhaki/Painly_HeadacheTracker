using CommunityToolkit.Maui;
using HeadacheTracker.Application.UseCases;
using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Infrastructure.Repositories;
using HeadacheTracker.Maui.Services;
using HeadacheTracker.Maui.ViewModels;
using HeadacheTracker.Maui.Views;

namespace HeadacheTracker.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "headache.db3");

            builder.Services.AddTransient<MainPage>();

            // Repositories           
            builder.Services.AddSingleton<IMedicationRepository>(new MedicationRepository(dbPath));
            builder.Services.AddSingleton<IHeadacheRepository>(sp =>
            {
                var medicationRepo = sp.GetRequiredService<IMedicationRepository>();
                return new HeadacheRepository(dbPath, medicationRepo);
            });
            // Use cases
            builder.Services.AddTransient<GetHeadachesByMonth>();

            // ViewModels
            builder.Services.AddTransient<CalendarViewModel>();
            builder.Services.AddTransient<AddEntryViewModel>();
            builder.Services.AddTransient<StatisticsViewModel>();
            builder.Services.AddTransient<StatisticsService>();


            // Popups (можно регистрировать, они сами подцепят DI)
            builder.Services.AddTransient<AddEntryPopup>();

            return builder.Build();
        }
    }
}
