using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;
using Firebase.Storage;

namespace CrossPlatformTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadPage : ContentPage
    {
        private static string dlURL;
        private static string fileID;
        public DownloadPage(string url, string filename)
        {
            dlURL = url;
            fileID = filename;
            NavigationPage.SetHasBackButton(this,false);
            InitializeComponent();
            
            GenBarcode.BarcodeValue = dlURL;
        }

        protected override async void OnDisappearing()
        {
                
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await Task.Delay(5000);
            await DeleteFiles();
            await this.DisplayAlert("Transfer Time Ended.", "Transfer has been terminated. Files have been deleted from server.", "OK");
            await this.Navigation.PopToRootAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                var result = await this.DisplayAlert("Terminate Transfer?", "Leaving this page will terminate the current transfer. Do you want to continue?", "Yes", "No");

                if (result)
                {
                    await DeleteFiles();
                    await this.Navigation.PopAsync();
                }
            });
            return true;
        }


        public async Task DeleteFiles()
        {
            await new FirebaseStorage("universalfiletransfer.appspot.com")
                .Child(Preferences.Get("my_deviceID", string.Empty))
                .Child(fileID)
                .DeleteAsync();
        }

        private void terminate_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }
}