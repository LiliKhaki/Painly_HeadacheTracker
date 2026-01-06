namespace HeadacheTracker.Maui.Models;

public class StatisticsSummary
{
    public int DaysWithPain { get; set; }
    public int PainEpisodes { get; set; }
    public double AverageIntensity { get; set; }
    public int PainFreeDays { get; set; }
    public double? AverageDose { get; set; }

    public int LongestPainFreeStreak { get; set; }
    public int DaysWithMedication { get; set; }
}
