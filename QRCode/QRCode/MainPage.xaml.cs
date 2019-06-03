using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace QRCode
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            


        }

        private async void Btn_Scan_Clicked(object obj)
        {
            Xamarin.Forms.DependencyService.Register<IQRCodeReader>();
            var data = await DependencyService.Get<IQRCodeReader>().ReadQRCode();

            Device.BeginInvokeOnMainThread(() =>
            {
                //Navigation.PopAsync();
                DisplayAlert("The text is : " + data, "", "OK");
            });
        }

        public async Task Navigate()
        {
            ZXing.Mobile.MobileBarcodeScanner option = new ZXing.Mobile.MobileBarcodeScanner();

            var scanner = new ZXing.Mobile.MobileBarcodeScanningOptions()
            {
                TryHarder = true,
                AutoRotate = false,
                TryInverted = true,
                DelayBetweenContinuousScans = 2000,
            };

            ZXingScannerPage scanPage = new ZXingScannerPage(scanner);
            
            await Navigation.PushAsync(scanPage);

            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;
                ZXing.BarcodeFormat barcodeFormat = result.BarcodeFormat;
                string type = barcodeFormat.ToString();
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    DisplayAlert("The Barcode type is : " + type, "The text is : " + result.Text, "OK");
                });
            };
        }
    }
}
