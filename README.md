##### Prequisites 
Set the api Url recieved from API gateway in the PushNotification.PushClient.PushNotificationClient
Note that the url will be like {baseUrl}/Api/

##### Setup Android
- *Install the following nuget packages*
    Xamarin.Firebase.Messaging
    Xamarin.GooglePlayServices.Base
    Xamarin.GooglePlayServices.Basement
  
- *Register app in FCM and copy the google-services.json. Set build action to  'GoogleServicesJson'*

- Give permission in menifest file

- *For new Users* : Override the FirebaseInstanceIdService OnTokenRefresh Method and call the registration
   (To be changed later). 
	* For old Users* : Once the user logs. Check the take the token from "FirebaseInstanceId.Instance.Token" 
	call the get endpoint to check if device token is registered.	
	If not  registered send the device registration call
  
 - *Implement the FirebaseMessagingService OnMessageRecieved to check if token is recieved 
   paste code sample here* 
   
  
  ##### Setup IOS
- *Follow instruction from https://docs.microsoft.com/en-us/azure/app-service-mobile/app-service-mobile-xamarin-forms-get-started-push
  to generate the certificates from APN*
- *Setup the certificates on visual studio using the above link*
- *Implement the AppDelegate and extend the FinishedLaunching method to register devices*
- *Implement the DidReceiveRemoteNotification method to recieve notfication*

