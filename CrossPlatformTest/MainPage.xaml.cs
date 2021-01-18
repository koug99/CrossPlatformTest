using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Firebase.Storage;
using System.Drawing;
using System.IO;
using System.Web;

namespace CrossPlatformTest
{
    public partial class MainPage : ContentPage
    {

        private static IEnumerable<FileResult> pickedFiles;
        
    
        public MainPage()
        {
            InitializeComponent();
            var deviceID = Preferences.Get("my_deviceID", string.Empty);
            if (string.IsNullOrWhiteSpace(deviceID))
            {
                
                deviceID = System.Guid.NewGuid().ToString();
                Preferences.Set("my_deviceID", deviceID);
            }
        }


        private async void ChooseFile_Pressed(object sender, EventArgs e)
        {
            pickedFiles = await FilePicker.PickMultipleAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Choose an image"
            }) ;

            if (pickedFiles != null)
            {
                foreach(var file in pickedFiles)
                {
                    file.ContentType = "application/x-www-form-urlencoded";
                    lbl.Text += file.FileName;
                }
            }
            else
            {
                await DisplayAlert("ERROR", "file not picked", "OK");
            }
        }
        
        

        private async void UploadButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string dlURL = null;
                foreach (var f in pickedFiles)
                {
                    var newstream = await f.OpenReadAsync();

                    dlURL += await UploadFile(newstream, f.FileName) + "\n";
                }

                dlURL = HttpUtility.HtmlEncode(dlURL);
                await DisplayAlert("Upload Successful!", dlURL , "Ok");
                await Navigation.PushAsync(new DownloadPage(dlURL));
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERROR", ex.ToString(), "Ok");
            }
            

            
        }
        public async Task<string> UploadFile(Stream fileStream, string filename)
        {

            var fileURL = await new FirebaseStorage("universalfiletransfer.appspot.com")
                .Child(Preferences.Get("my_deviceID", string.Empty))
                .Child(filename)
                .PutAsync(fileStream);

            return fileURL;
        }
    }

}
