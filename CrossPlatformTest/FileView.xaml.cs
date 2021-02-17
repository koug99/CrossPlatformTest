using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;


using Firebase.Storage;
using System.IO;
using System.Web;
using System.IO.Compression;
using System.Threading;

namespace CrossPlatformTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FileView : ContentPage
    {
        public static string fileID;

        public FileView( IEnumerable<FileResult> pF)
        {
            InitializeComponent();
            BindingContext = this;
            this.pickedFiles = pF;
        }

        private IEnumerable<FileResult> pickedFiles;
        public IEnumerable<FileResult> Files
        {
            get => pickedFiles;
        }

        private async void UploadButton_Clicked(object sender, EventArgs e)
        {
            try
            {
                string dlURL = null;

                var cacheDir = FileSystem.CacheDirectory;
                var zipPath = $"FileTransfer_{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.zip";
                List<string> filepath = new List<string>();
                string mimetype = null;

                foreach (var f in pickedFiles)
                {
                    filepath.Add(f.FullPath);
                    mimetype = f.ContentType; //Sets file MIME for single file selections
                }

                if (filepath.Count > 1)
                {
                    fileID = zipPath;
                    ZipAndShip(filepath, cacheDir, zipPath);
                    FileStream fs = File.OpenRead(cacheDir + zipPath);
                    dlURL = await UploadFile(fs, fileID, "application/zip");
                    fs.Close();
                }
                else
                {

                    fileID = filepath[0].Substring(filepath[0].LastIndexOf("/") + 1);
                    FileStream fs = File.OpenRead(filepath[0]);
                    dlURL = await UploadFile(fs, fileID, mimetype);
                    fs.Close();
                }
                dlURL = HttpUtility.HtmlEncode(dlURL);
                await DisplayAlert("Upload Successful!", "QR Code generated for sharing.", "Ok");

                await Navigation.PushAsync(new DownloadPage(dlURL, fileID));
            }
            catch (Exception ex)
            {
                await DisplayAlert("No File Found", "Please select a file to upload", "Ok");
            }



        }
        public async Task<string> UploadFile(Stream fileStream, string filename, string mime)
        {
            CancellationToken token = new CancellationTokenSource().Token;


            var fileURL = await new FirebaseStorage("universalfiletransfer.appspot.com")
                .Child(Preferences.Get("my_deviceID", string.Empty))
                .Child(filename)
                .PutAsync(fileStream, token, mime);

            return fileURL;
        }

        public void ZipAndShip(List<string> filecollection, string zipDir, string zipName)
        {
            if (Directory.Exists(zipDir))
            {
                try
                {

                    using (ZipArchive files = ZipFile.Open(zipDir + zipName, ZipArchiveMode.Create))
                    {
                        foreach (var f in filecollection)
                        {
                            files.CreateEntryFromFile(f, Path.GetFileName(f), CompressionLevel.Optimal);
                        }
                    }


                }
                catch (Exception e)
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
