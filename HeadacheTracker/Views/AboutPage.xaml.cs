using HeadacheTracker.Maui.Resources.Strings;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using System.Windows.Input;


namespace HeadacheTracker.Maui.Views
{
    public partial class AboutPage : ContentPage
    {
        public string AppVersion => $"{AppResources.Version_Label} {AppInfo.Current.VersionString}";
        public ICommand OpenSupportCommand { get; set;  }
        public ICommand OpenBugReportFormCommand { get; set; }


        public AboutPage()
        {
            InitializeComponent();
           

            System.Diagnostics.Debug.WriteLine("binding context: " + BindingContext?.GetType().ToString());
            System.Diagnostics.Debug.WriteLine(OpenSupportCommand == null ? "NULL" : "NOT NULL");

            OpenSupportCommand = new Command(async () =>
            {
                try
                {
                    var url = new Uri("https://www.paypal.com/donate/?hosted_button_id=8CTZ7EZ33V2X2\r\n");

                    await Launcher.Default.OpenAsync(url);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[Support] {ex}");
                    var page = Microsoft.Maui.Controls.Application.Current?.Windows.FirstOrDefault()?.Page;
                    if (page == null)
                    {
                        System.Diagnostics.Debug.WriteLine("no active Page found.");
                        return;
                    }
                    await page.DisplayAlert(
                        AppResources.Error,
                        AppResources.UnableToOpenDonationPage,
                        AppResources.OkButton);
                }
            });

            OpenBugReportFormCommand = new Command(async () =>
            {
                var url = "https://forms.gle/JnRgEQLEkzvy2mBU7";
                await Launcher.OpenAsync(url);
            });

            BindingContext = this;

        }


    }
}
