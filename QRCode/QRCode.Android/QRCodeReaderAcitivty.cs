using System;

using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Content.PM;
using Android.Graphics;
using Android.Gms.Vision.Barcodes;
using Android.Gms.Vision;
using Android.Support.V4.App;
using Android;
using static Android.Gms.Vision.Detector;
using Android.Util;
using System.Threading.Tasks;
using Xamarin.Forms;
using QRCode.Droid;
using Android.Content;

namespace QRCode.Droid
{
    [Activity(Label = "QRCodeReaderAcitivty", Theme = "@style/MainTheme", ScreenOrientation = ScreenOrientation.Portrait, ConfigurationChanges = (ConfigChanges.Orientation
           | ConfigChanges.ScreenSize | ConfigChanges.KeyboardHidden))]
    public class QRCodeReaderAcitivty : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, ISurfaceHolderCallback, IProcessor
    {
        #region QR Reader
        SurfaceView cameraPreview;
        BarcodeDetector barcodeDetector;
        CameraSource cameraSource;
        const int RequestCameraPermissionID = 1001;
        string result;
        #endregion
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestCameraPermissionID:
                    {
                        if (grantResults[0] == Permission.Granted)
                        {
                            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                            {
                                //Request permission
                                ActivityCompat.RequestPermissions(this, new string[]
                                {
                                    Manifest.Permission.Camera
                                }, RequestCameraPermissionID);
                                return;
                            }
                            try
                            {
                                cameraSource.Start(cameraPreview.Holder);
                            }
                            catch (InvalidOperationException)
                            {

                            }
                        }
                    }
                    break;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            StartScanning();
        }

        #region IQRCodeReader Impletation
        /*public async Task<string> ReadQRCode()
        {
            await StartScanning();
            return result;
        }*/
        #endregion

        public async Task StartScanning()
        {

            //cameraPreview = FindViewById<SurfaceView>(Resource.Id.cameraPreview);

            cameraPreview = new SurfaceView(this);
            SetContentView(cameraPreview);

            barcodeDetector = new BarcodeDetector.Builder(this)
                .SetBarcodeFormats(BarcodeFormat.QrCode)
                .Build();
            cameraSource = new CameraSource
                .Builder(this, barcodeDetector)
                .SetRequestedPreviewSize(640, 480)
                .SetAutoFocusEnabled(true)
                .Build();

            

            cameraPreview.Holder.AddCallback(this);
            barcodeDetector.SetProcessor(this);
        }

        #region ISurfaceHolderCallback Implmentation
        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                //Request permission
                ActivityCompat.RequestPermissions(this, new string[]
                {
                   Manifest.Permission.Camera
                }, RequestCameraPermissionID);
                return;
            }
            try
            {
                cameraSource.Start(cameraPreview.Holder);
            }
            catch (InvalidOperationException)
            {

            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
        }
        #endregion

        #region IProcessor Implementation
        public void ReceiveDetections(Detections detections)
        {
            SparseArray qrcodes = detections.DetectedItems;
            if (qrcodes.Size() != 0)
            {
                result = ((Barcode)qrcodes.ValueAt(0)).RawValue;
                Intent resultIntent = new Intent(this, typeof(MainActivity));
                resultIntent.PutExtra("json", ((Barcode)qrcodes.ValueAt(0)).RawValue);
                SetResult(Result.Ok, resultIntent);
                Finish();
            }
        }

        public void Release()
        {

        }
        #endregion
    }
}