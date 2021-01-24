using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Firebase;
using Firebase.Storage; 
using System.Drawing;
using System.IO;
using System.Web;
using Java.Util.Zip;

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
            FilePickerFileType fileType = null;


            if(sender.Equals(img))
            {
                fileType = FilePickerFileType.Images;
            }
            else if(sender.Equals(vid))
            {
                fileType = FilePickerFileType.Videos;
            }

            int index = 0;
            pickedFiles = await FilePicker.PickMultipleAsync(new PickOptions
            {
                FileTypes = fileType,
                PickerTitle = "Choose a file"
            }) ;

            if (pickedFiles != null)
            {
                foreach(var file in pickedFiles)
                {
                    lbl.Text += file.FullPath;
                    index++;
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

                var cacheDir = FileSystem.CacheDirectory;
                var zipPath = $"FileTransfer_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.zip";
                List<string> filepath = new List<string>();

                foreach(var f in pickedFiles)
                {
                    filepath.Add(f.FullPath);
                }

                if (filepath.Count > 1)
                {
                    ZipAndShip(filepath, cacheDir, zipPath);
                    FileStream fs = File.OpenRead(cacheDir + zipPath);
                    dlURL = await UploadFile(fs, zipPath);
                    fs.Close();
                }
                else
                {
                    FileStream fs = File.OpenRead(filepath[0]);
                    dlURL = await UploadFile(fs, filepath[0].Substring(filepath[0].LastIndexOf("/") + 1));
                    fs.Close();
                }
                dlURL = HttpUtility.HtmlEncode(dlURL);
                await DisplayAlert("Upload Successful!", dlURL, "Ok");
                await Navigation.PushAsync(new DownloadPage(dlURL));
}
            catch (Exception ex)
            {
                await DisplayAlert("No File Found", "Please select a file to upload", "Ok");
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

        public void ZipAndShip(List<string> filecollection, string zipDir, string zipPath)
        {
            if (Directory.Exists(zipDir))
            {
                FileStream fos;
                ZipOutputStream zos;
                try
                {
                    fos = new FileStream(zipDir + zipPath, FileMode.Create);
                    zos = new ZipOutputStream(fos);

                    foreach (var f in filecollection)
                    {
                        
                        ZipEntry zipe = new ZipEntry(f.Substring(f.LastIndexOf("/") + 1));
                        zos.PutNextEntry(zipe);

                        byte[] filedata = File.ReadAllBytes(f);
                        zos.Write(filedata);
                        zos.CloseEntry();

                    }
                    zos.Close();
                    fos.Close();
                    //zos.Close();

                }
                catch(Exception e)
                {
                    DisplayAlert("ERROR", e.ToString(), "Ok");
                }

            }
            else
            {
                DisplayAlert("ERROR", "Directory does not exist", "Ok");
            }
        }
    }

}
