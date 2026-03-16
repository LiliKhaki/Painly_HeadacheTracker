using CommunityToolkit.Maui;
using HeadacheTracker.Application.UseCases;
using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Infrastructure;
using HeadacheTracker.Infrastructure.Repositories;
using HeadacheTracker.Maui.Services;
using HeadacheTracker.Maui.ViewModels;
using HeadacheTracker.Maui.Views;
using Microsoft.Maui.Controls.Handlers.Items;
using SQLite;


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

            builder.Services.AddSingleton<SQLiteAsyncConnection>(sp =>
            {
                return new SQLiteAsyncConnection(dbPath);
            });
            builder.Services.AddSingleton<DatabaseInitializer>();


            // Repositories           
            builder.Services.AddSingleton<IMedicationRepository, MedicationRepository>();
            builder.Services.AddSingleton<IHeadacheRepository, HeadacheRepository>();

            // Use cases
            builder.Services.AddTransient<GetHeadachesByMonth>();

            // Pages
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<StatisticsPage>();
            builder.Services.AddTransient<AboutPage>();

            // ViewModels
            builder.Services.AddSingleton<CalendarViewModel>();
            builder.Services.AddTransient<AddEntryViewModel>();
            builder.Services.AddSingleton<StatisticsViewModel>();
            builder.Services.AddTransient<StatisticsService>();

            // Popups
            builder.Services.AddTransient<AddEntryPopup>();

            CollectionViewHandler.Mapper.AppendToMapping("Optimize", (handler, view) => { });

            return builder.Build();
        }
    }
}
