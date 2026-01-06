using CommunityToolkit.Maui.Views;
using HeadacheTracker.Maui.ViewModels;
using Microsoft.Maui.Devices;
using System;
using System.Diagnostics;

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

        // Устанавливаем BindingContext
        BindingContext = viewModel;

        // Заголовок
        Title = viewModel.IsEditMode ? "Edit Headache Record" : "Headache Record";

        // Слушаем событие закрытия
        viewModel.RequestClose += OnRequestClose;

        // Настройка адаптивного размера
        SetAdaptiveSize();
    }

    private async void OnRequestClose()
    {
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

            var width = screenWidthDp * 0.92;   // 92% ширины
            var maxHeight = screenHeightDp * 0.86; // 86% высоты

            this.WidthRequest = width;
            this.HeightRequest = maxHeight;

            // Находим RootLayout по имени
            var rootLayout = this.FindByName<VerticalStackLayout>("RootLayout");
            if (rootLayout != null)
            {
                rootLayout.WidthRequest = width - (rootLayout.Padding.Left + rootLayout.Padding.Right);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[AddEntryPopup] SetAdaptiveSize failed: {ex}");
        }
    }

}
