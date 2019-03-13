using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Util;
using Firebase.Iid;
using RestSharp;

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
            Task.Run(async () => await SendRegistrationToServer(refreshedToken));
        }
        async Task SendRegistrationToServer(string tokenString)
        {       
                // Send the jwtToken here.
                var authToken = "user1";
                var client = new RestClient(apiUrl);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("Authorization", "user1");
                request.AddHeader("Content-Type", "application/json");
                var token = new
                {
                    deviceInfo = new
                    {
                        // Idealy user id should be the logged in user.
                        userId = Guid.NewGuid().ToString(),
                        deviceToken = tokenString,
                        deviceType = "2"
                    }
                };

                var body = Newtonsoft.Json.JsonConvert.SerializeObject(token);

                request.AddParameter("undefined", body, ParameterType.RequestBody);
               var response =  await client.ExecuteTaskAsync(request);
        }
    }
}