using AzureADAuthenticationUtilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBusListener
{
    public static class SignalRNotifiationHandler
    {
        public static async Task PushNotification(string message)
        {
            try
            {
                await PushNotificationAKSHostedFrontend(message); // Required for container based deployment
                message = DateTime.Now.ToString("HH:mm:ss.FFF") + "  =>  " + message;
                var tokens = new string[3] { "30832cc6-3592-45be-bde4-405e5f00a615", "_1XdTt0j~9xH-c9-EY5mBUK~12M4s-8YDj", "38857842-8570-4fcf-870b-b7aa1fcddf06" };
                string aadInstance = "https://login.windows.net/{0}";
                string tenant = tokens[2];
                string serviceResourceId = string.Format("api://{0}", tokens[0]);
                string clientId = tokens[0];
                string appKey = tokens[1];

                // Get auth token and add the access token to the authorization header of the request.
                var httpClient = new HttpClient();
                var authContext = new AuthenticationContext(string.Format(CultureInfo.InvariantCulture, aadInstance, tenant));
                var clientCredential = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(clientId, appKey);
                Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult result = await authContext.AcquireTokenAsync(serviceResourceId, clientCredential);
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

               
                var content = new StringContent(message, Encoding.UTF8, "application/json");
                
                // Send the request and read the response
                await httpClient.PostAsync(
                    string.Format("https://g-works-signalrserver.azurewebsites.net/api/PostSignalRMessageToUser/{0}", "viewer01@priyankomukherjeegmail.onmicrosoft.com"), content);
            }
            catch (Exception ex)
            {
                var str = ex.Message;
            }

        }

        public static async Task PushNotificationAKSHostedFrontend(string message)
        {
            TableStorageWriter writer = new TableStorageWriter();
            try
            {
                message = DateTime.Now.ToString("HH:mm:ss.FFF") + "  =>  " + message;
                var tokens = new string[3] { "50e6b9f0-b53e-42c7-89ab-bcbf35c47147", "_q4p5[BZ5td3xXdzWNDPNLuHapVCVG_.", "38857842-8570-4fcf-870b-b7aa1fcddf06" };
                string aadInstance = "https://login.windows.net/{0}";
                string tenant = tokens[2];
                string serviceResourceId = string.Format("api://{0}", tokens[0]);
                string clientId = tokens[0];
                string appKey = tokens[1];

                
               // ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
                using (var httpClientHandler = new HttpClientHandler())
                {
                    httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                    
                    // Get auth token and add the access token to the authorization header of the request.
                    var httpClient = new HttpClient(httpClientHandler);
                    var authContext = new AuthenticationContext(string.Format(CultureInfo.InvariantCulture, aadInstance, tenant));
                    var clientCredential = new Microsoft.IdentityModel.Clients.ActiveDirectory.ClientCredential(clientId, appKey);
                    Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult result = await authContext.AcquireTokenAsync(serviceResourceId, clientCredential);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);

                    CustomMessage objMessage = new CustomMessage() { Message = message };

                    using (var request = new HttpRequestMessage(HttpMethod.Post, "http://gatewayapi/signalrhub"))
                    {
                        var json = JsonConvert.SerializeObject(objMessage);
                        using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                        {
                            request.Content = stringContent;

                            using (var response = await httpClient
                                .SendAsync(request)
                                .ConfigureAwait(false))
                            {
                                response.EnsureSuccessStatusCode();
                            }
                        }
                    }

                }
               
            }
            catch (Exception ex)
            {
                var str = ex.Message;
                writer.Write("Signarprocessor error: " + str);
            }

        }
    }

    public class CustomMessage
    {
        public string Message { get; set; }
    }
}
