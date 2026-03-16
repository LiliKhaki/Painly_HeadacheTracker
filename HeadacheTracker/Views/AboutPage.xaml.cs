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
                System.Diagnostics.Debug.WriteLine("Command fired!");
                var url = "https://www.paypal.me/LiliyaKhaki"; 
                if (await Launcher.CanOpenAsync(url))
                    await Launcher.OpenAsync(url);
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
