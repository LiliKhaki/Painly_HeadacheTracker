using CommunityToolkit.Maui.Views;
using HeadacheTracker.Maui.ViewModels;
using Microsoft.Maui.Devices;
using System;
using System.Diagnostics;

#if ANDROID
using Android.Content;
using Android.Views;
using Android.Views.InputMethods;
using Microsoft.Maui.Platform;
#endif

namespace HeadacheTracker.Maui.Views;

public partial class AddEntryPopup : Popup
{
    public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(nameof(Title), typeof(string), typeof(AddEntryPopup), default(string));

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public AddEntryPopup(AddEntryViewModel viewModel)
    {
        InitializeComponent();

        BindingContext = viewModel;
        
        // Заголовок
        Title = viewModel.IsEditMode ? "Edit Headache Record" : "Headache Record";

        // Слушаем событие закрытия
        viewModel.RequestClose += OnRequestClose;

        // Настройка адаптивного размера
        SetAdaptiveSize();      
    }

    async void OnRequestClose()
    {
        
        RootLayout.Focus();
        NotesEditor?.Unfocus();

        Debug.WriteLine("[AddEntryPopup] OnRequestClose received");
        await CloseAsync();
        Debug.WriteLine("[AddEntryPopup] CloseAsync finished");
    }

    private void SetAdaptiveSize()
    {
        try
        {
            var main = DeviceDisplay.MainDisplayInfo;
            var screenWidthDp = main.Width / main.Density;
            var screenHeightDp = main.Height / main.Density;

            var width = screenWidthDp * 0.92;    // 92% ширины
            var maxHeight = screenHeightDp * 0.86; // 86% высоты

            this.WidthRequest = width;
            this.HeightRequest = maxHeight;

            if (RootLayout != null)
            {
                RootLayout.WidthRequest = width - (RootLayout.Padding.Left + RootLayout.Padding.Right);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[AddEntryPopup] SetAdaptiveSize failed: {ex}");
        }
    }

    /// <summary>
    /// Вызывается при нажатии Done на Entry медикаментов
    /// </summary>
    private void OnDoseCompleted(object sender, EventArgs e)
    {
        if (sender is Entry entry)
            entry.Unfocus();
    }

    private void OnBackgroundTapped(object sender, EventArgs e)
    {
        
        RootLayout.Focus();
        NotesEditor?.Unfocus();
    }

    private void OnEntryCompleted(object sender, EventArgs e)
    {
        if (sender is Entry entry)
            entry.Unfocus();
    }

}
