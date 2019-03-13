using RestSharp;
using System;
using System.Threading.Tasks;

namespace PushNotification.PushClient
{
    public class PushNotificationClient
    {
        /// <summary>
        /// Add the api url here.
        /// </summary>
        private const string apiUrl = "";
        const string deviceRoute = @"/api/device";

        public async Task RegiterDevice(string jwtToken, RegisterDeviceRequest deviceRequest)
        {       
            // Send the jwtToken here.
            var authToken = jwtToken;
            var client = new RestClient(apiUrl + deviceRoute);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Authorization", authToken);
            request.AddHeader("Content-Type", "application/json");
           

            var body = Newtonsoft.Json.JsonConvert.SerializeObject(deviceRequest);

            request.AddParameter("undefined", body, ParameterType.RequestBody);
            var response = await client.ExecuteTaskAsync(request);
        }
    }
}
