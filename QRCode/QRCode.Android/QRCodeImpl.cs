using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QRCode.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(QRCodeImpl))]
namespace QRCode.Droid
{
    public class QRCodeImpl : IQRCodeReader
    {
        public Task<string> ReadQRCode()
        {
            string result;

            var activity = (MainActivity)Forms.Context;
            var listener = new ActivityResultListener(activity);

            const int RequestEnableBt = 2;
            //var intent = new Intent(BluetoothAdapter.ActionRequestEnable);
            var intent = new Intent(Forms.Context, typeof(QRCodeReaderAcitivty));
            activity.StartActivityForResult(intent, RequestEnableBt);

            return listener.Task;

            /*var intent = new Intent(Forms.Context, typeof(QRCodeReaderAcitivty));
            Forms.Context.StartActivity (intent);
            ((Activity)Forms.Context).StartActivityForResult(intent,101);
            return null;*/
        }

        private class ActivityResultListener
        {
            private TaskCompletionSource<string> Complete = new TaskCompletionSource<string>();
            public Task<string> Task { get { return this.Complete.Task; } }

            public ActivityResultListener(MainActivity activity)
            {
                activity.ActivityResult += OnActivityResult;
            }

            private void OnActivityResult(int requestCode, Result resultCode, Intent data)
            {
                // unsubscribe from activity results
                var context = Forms.Context;
                var activity = (MainActivity)context;
                activity.ActivityResult -= OnActivityResult;

                // process result
                if (resultCode != Result.Ok)
                    this.Complete.TrySetResult("");
                else
                    this.Complete.TrySetResult(data.GetStringExtra("json"));
            }
        }
            
    }
}