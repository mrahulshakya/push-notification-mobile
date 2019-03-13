namespace PushNotification.PushClient
{
    public class DeviceInfo
    {
        /// <summary>
        /// Gets or sets the userId which is using the device.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the device token information.
        /// </summary>
        public string DeviceToken { get; set; }

        /// <summary>
        ///  0 = unknown
        ///  1 = IOS ,
        ///  2 = Android,
        ///  3 = Web
        /// </summary>
        public DeviceType DeviceType { get; set; }
    }
}
