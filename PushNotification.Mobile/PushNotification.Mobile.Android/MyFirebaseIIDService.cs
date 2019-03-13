using Android.App;
using Android.Util;
using Firebase.Iid;
using PushNotification.PushClient;
using System;
using System.Threading.Tasks;

namespace PushNotification.Mobile.Droid
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        /// <summary>
        /// Please note that device type = 1 for IOS and 2 for android.
        /// </summary>
        const int deviceType = 2;
        const string TAG = "MyFirebaseIIDService";

        // Add your gateway Url.
        const string apiUrl = @"";
        public override void OnTokenRefresh()
        {
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);

            var client = new PushNotificationClient();
            var jwtToken = "user1";
            var request = new RegisterDeviceRequest
            {
                DeviceInfo = new DeviceInfo
                {
                    DeviceToken = refreshedToken,
                    DeviceType = DeviceType.Android,
                    UserId = Guid.NewGuid().ToString() // Replace this with the logged in user id.
                }
            };

            Task.Run(async () => await client.RegiterDevice(jwtToken,  request));
        }
       
    }
}