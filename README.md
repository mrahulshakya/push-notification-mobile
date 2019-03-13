
##### **Prequisites**
Set the api Url recieved from API gateway in the PushNotification.PushClient.PushNotificationClient
Note that the url will be like {baseUrl}/Api/

##### **Setup Android**
- **Install the following nuget packages**
 Xamarin.Firebase.Messaging
 Xamarin.GooglePlayServices.Base
 Xamarin.GooglePlayServices.Basement

- ***Register app in FCM and copy the google-services.json. Set build action to  'GoogleServicesJson'***

- **Give permission in menifest file**
```xml
<application android:label="PushNotification.Mobile.Android">
    <receiver
      android:name="com.google.firebase.iid.FirebaseInstanceIdInternalReceiver"
      android:exported="false" />
    <receiver
        android:name="com.google.firebase.iid.FirebaseInstanceIdReceiver"
        android:exported="true"
        android:permission="com.google.android.c2dm.permission.SEND">
      <intent-filter>
        <action android:name="com.google.android.c2dm.intent.RECEIVE" />
        <action android:name="com.google.android.c2dm.intent.REGISTRATION" />
        <category android:name="${applicationId}" />
      </intent-filter>
    </receiver>
    <meta-data
    android:name="com.google.firebase.messaging.default_notification_icon"
    android:resource="@drawable/ic_stat_ic_notification" />
	</application>
```
- **For new Users : Override the FirebaseInstanceIdService OnTokenRefresh Method and call the registration**
```csharp
 [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        /// <summary>
        /// Please note that device type = 1 for IOS and 2 for android.
        /// </summary>
        const int deviceType = 2;
        const string TAG = "MyFirebaseIIDService";

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
```
	**For old Users** : Once the user logs. Check the take the token from "FirebaseInstanceId.Instance.Token" 
	call the get endpoint to check if device token is registered.	
	If not  registered send the device registration call


 - **Implement the FirebaseMessagingService OnMessageRecieved to check if token is recieved **
 ```csharp
 public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, "From: " + message.From);

            var notification = message.GetNotification();
            Log.Debug(TAG, "Notification Message Body: " + notification);
            SendNotification(notification, message.Data);
        }

        void SendNotification(RemoteMessage.Notification notification, IDictionary<string, string> data)
        {
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            foreach (var key in data.Keys)
            {
                intent.PutExtra(key, data[key]);
            }

            var pendingIntent = PendingIntent.GetActivity(this,
                                                          MainActivity.NOTIFICATION_ID,
                                                          intent,
                                                          PendingIntentFlags.OneShot);

            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.CHANNEL_ID)
                                      .SetSmallIcon(Resource.Drawable.ic_stat_ic_notification)
                                      .SetContentTitle(notification.Title)
                                      .SetContentText(notification.Body)
                                      .SetAutoCancel(true)
                                      .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(MainActivity.NOTIFICATION_ID, notificationBuilder.Build());
        }
`
 #####  **Setup IOS**
- **Follow instruction from https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-xamarin-forms-get-started-push
  to generate the certificates from APN**
- **Setup the certificates on visual studio using the above link**
- **Implement the AppDelegate and extend the FinishedLaunching method to register devices
**
```csharp
 public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
   
            // Register for push notifications.
            var settings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert
                | UIUserNotificationType.Badge
                | UIUserNotificationType.Sound,
                new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            return base.FinishedLaunching(app, options);

        }

        public override void RegisteredForRemoteNotifications(UIApplication application,
    NSData token)
        {
            //Note that the device type should be 1 in case of IOS devices.
            var deviceToken = token.Description.Replace("<", "").Replace(">", "").Replace(" ", "");
            if (!string.IsNullOrEmpty(deviceToken))
            {

                var client = new PushNotificationClient();
                var jwtToken = "user1";
                var request = new RegisterDeviceRequest
                {
                    DeviceInfo = new DeviceInfo
                    {
                        DeviceToken = deviceToken,
                        DeviceType = DeviceType.Android,
                        UserId = Guid.NewGuid().ToString() // Replace this with the logged in user id.
                    }
                };

                Task.Run(async () => await client.RegiterDevice(jwtToken, request));
            }

        }

```
- **Implement the DidReceiveRemoteNotification method to recieve notfication**
```csharp
 public override void DidReceiveRemoteNotification(UIApplication application,
    NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

            string alert = string.Empty;
            if (aps.ContainsKey(new NSString("alert")))
                alert = (aps[new NSString("alert")] as NSString).ToString();

            //show alert
            if (!string.IsNullOrEmpty(alert))
            {
                UIAlertView avAlert = new UIAlertView("Notification", alert, null, "OK", null);
                avAlert.Show();
            }
        }
```
