using System;
using Microsoft.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace HeadacheTracker.Maui.Helpers
{
    public static class ServiceHelper
    {
        public static TService GetService<TService>() where TService : notnull =>
            Current.GetRequiredService<TService>();

        public static IServiceProvider Current =>
            Microsoft.Maui.Controls.Application.Current?.Handler?.MauiContext?.Services
            ?? throw new InvalidOperationException("ServiceProvider not available");
    }
}
