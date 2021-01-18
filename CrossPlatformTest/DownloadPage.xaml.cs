using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.IO;

namespace CrossPlatformTest
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadPage : ContentPage
    {
        private static string dlURL;
        public DownloadPage(string url)
        {
            dlURL = url;
            InitializeComponent();

            GenBarcode.BarcodeValue = dlURL;
        }
        
    }
}