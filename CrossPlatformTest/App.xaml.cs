using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace CrossPlatformTest
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            var deviceID = Preferences.Get("my_deviceID", string.Empty);
            if (string.IsNullOrWhiteSpace(deviceID))
            {

                deviceID = System.Guid.NewGuid().ToString();
                Preferences.Set("my_deviceID", deviceID);
            }
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            var cacheDir = FileSystem.CacheDirectory;
        }

        protected override void CleanUp()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}