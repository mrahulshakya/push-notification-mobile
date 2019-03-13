using System;
using System.Threading.Tasks;
using Foundation;
using PushNotification.PushClient;
using RestSharp;
using UIKit;

namespace PushNotification.Mobile.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
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
    }
}
