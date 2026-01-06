using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using HeadacheTracker.Application.UseCases;
using HeadacheTracker.Domain.Abstractions;
using HeadacheTracker.Domain.Entities;
using HeadacheTracker.Infrastructure.Repositories;
using HeadacheTracker.Maui.Messages;
using HeadacheTracker.Maui.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Formats.Tar;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace HeadacheTracker.Maui.ViewModels
{


    public class CalendarDay : ObservableObject
    {
        public DateTime Date { get; }

        private bool _isToday;
        public bool IsToday
        {
            get => _isToday;
            set => SetProperty(ref _isToday, value);
        }

        private bool _hasEntry;
        public bool HasEntry
        {
            get => _hasEntry;
            set => SetProperty(ref _hasEntry, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private bool _isFromCurrentMonth;
        public bool IsFromCurrentMonth
        {
            get => _isFromCurrentMonth;
            set => SetProperty(ref _isFromCurrentMonth, value);
        }

        public CalendarDay(DateTime date)
        {
            Date = date;
        }
    }
}
