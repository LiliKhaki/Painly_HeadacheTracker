using System;
using Microsoft.Maui.Controls;

namespace HeadacheTracker.Maui.Helpers
{
    public static class ServiceHelper
    {
        public static TService GetService<TService>() =>
            Current.GetService<TService>();

        public static IServiceProvider Current => Microsoft.Maui.Controls.Application.Current!.Handler!.MauiContext!.Services;
    }
}
