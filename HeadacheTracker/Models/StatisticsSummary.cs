using CommunityToolkit.Mvvm.ComponentModel;

namespace HeadacheTracker.Maui.Models;

public partial class StatisticsSummary : ObservableObject
{
    [ObservableProperty] private int daysWithPain;
    [ObservableProperty] private int painEpisodes;
    [ObservableProperty] private double averageIntensity;
    [ObservableProperty] private int painFreeDays;
    [ObservableProperty] private int longestPainFreeStreak;
    [ObservableProperty] private int daysWithMedication;
    [ObservableProperty] private double? averageDose;
}
