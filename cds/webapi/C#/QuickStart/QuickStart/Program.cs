using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Newtonsoft.Json;

namespace DataVerse
{
    class Program
    {
        static void Main()
        {
            // TODO Specify the Dataverse environment name to connect with.
            string resource = "https://econdev-cms-prod.crm9.dynamics.com/";
            //string resource = "https://<env-name>.api.<region>.dynamics.com";

            // Azure Active Directory app registration shared by all Power App samples.
            // For your custom apps, you will need to register them with Azure AD yourself.
            // See https://docs.microsoft.com/powerapps/developer/data-platform/walkthrough-register-app-azure-active-directory
            var clientId = "9d221412-bbf5-4b5b-b571-369d71ef6f26";
            var clientSecret = "6V~8Q~n6enqwoMvYSjYYH7k7nsFw7PbZNL-mOb2B";
            var redirectUri = new Uri("https://econdev-cms-prod.crm9.dynamics.com/");

            #region Authentication

            // The authentication context used to acquire the web service access token
            var authContext = new AuthenticationContext(
                "https://login.microsoftonline.com/a5cbe45f-1120-441c-a6e7-864e2c77e8c4");

            //var authContext = new AuthenticationContext(
            //    "https://login.microsoftonline.com/common", false);


            ClientCredential clientCredential = new ClientCredential(clientId, clientSecret);

            AuthenticationResult token = authContext.AcquireTokenAsync(resource, clientCredential).GetAwaiter().GetResult();

            string accessToken = token.AccessToken;
            // Get the web service access token. Its lifetime is about one hour after
            // which it must be refreshed. For this simple sample, no refresh is needed.
            // See https://docs.microsoft.com/powerapps/developer/data-platform/authenticate-oauth
            //var token = authContext.AcquireTokenAsync(
            //    resource, clientId, redirectUri,
            //    new PlatformParameters(
            //        PromptBehavior.Never   // Prompt the user for a logon account.
            //    ),
            //    UserIdentifier.AnyUser
            //).Result;

            #endregion Authentication

            #region Client configuration

            var client = new HttpClient
            {
                // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors#web-api-url-and-versions
                BaseAddress = new Uri(resource + "/api/data/v9.2/"),
                Timeout = new TimeSpan(0, 2, 0)    // Standard two minute timeout on web service calls.
            };

            // Default headers for each Web API call.
            // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors#http-headers
            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");
            headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            #endregion Client configuration

            #region Web API call

            // Invoke the Web API 'WhoAmI' unbound function.
            // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/compose-http-requests-handle-errors
            // See https://docs.microsoft.com/powerapps/developer/data-platform/webapi/use-web-api-functions#unbound-functions
            var response = client.GetAsync("WhoAmI").Result;

            if (response.IsSuccessStatusCode)
            {
                // Parse the JSON formatted service response to obtain the user ID.  
                JObject body = JObject.Parse(
                    response.Content.ReadAsStringAsync().Result);
                Guid userId = (Guid)body["UserId"];

                Console.WriteLine("Your user ID is {0}", userId);
                Console.WriteLine("Now let's get some data!");
            }
            else
            {
                Console.WriteLine("Web API call failed");
                Console.WriteLine("Reason: " + response.ReasonPhrase);
            }

            var dvUrl = "https://econdev-cms-prod.crm9.dynamics.com/api/data/v9.0/accounts?$select=name,accountnumber&$top=3";
            //var dvResult = 
            //dvUrl
            //.WithHeader("Authorization", token.AccessToken)
            //.GetStringAsync().Result;
            var dvResponse = client.GetAsync("accounts?$select=name,accountnumber&$top=100").Result;
            if (dvResponse.IsSuccessStatusCode)
            {
                // Parse the JSON formatted service response to obtain the user ID.  
                //JObject body = JObject.Parse(
                //    dvResponse.Content.ReadAsStringAsync().Result);
                //var values = body["value"];
                var dvString = dvResponse.Content.ReadAsStringAsync().Result;
                var odata = JsonConvert.DeserializeObject<OData>(dvString);


                Console.WriteLine(dvString.ToString());
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Web API call failed");
                Console.WriteLine("Reason: " + response.ReasonPhrase);
            }

        

            #endregion Web API call

            // Pause program execution by waiting for a key press.
            Console.ReadKey();
        }
    }
}
