using System.Net.Http.Headers;
using Microsoft.Identity.Client;
namespace SecureClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Making the call...");
            RunAsync().GetAwaiter().GetResult();
        }

        private static async Task RunAsync()
        {
            AuthConfig config = AuthConfig.ReadJsonFromFile("appsettings.json");

            Console.WriteLine($"Authority: {config.Authority}");

            IConfidentialClientApplication app;

            app = ConfidentialClientApplicationBuilder.Create(config.ClientId)
                .WithClientSecret(config.ClientSecret)
                .WithAuthority(new Uri(config.Authority))
                .Build();

            string[] ResourceIds = new string[] { config.ResourceId };

            AuthenticationResult? result = null;

            try
            {
                // Acquire Azure Active Directory (AAD) token 
                result = await app.AcquireTokenForClient(ResourceIds).ExecuteAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Token aquired \n");
                Console.WriteLine(result.AccessToken);
                Console.ResetColor();
            }
            catch (MsalClientException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine(e.Message);
                Console.ResetColor();
            }

            if (result != null && !string.IsNullOrEmpty(result.AccessToken))
            {
                // Create HTTP client with default request header
                var httpClient = new HttpClient();
                var defaultRequestHeaders = httpClient.DefaultRequestHeaders;

                // Make sure application/json is included in the Accept request HTTP header
                if (defaultRequestHeaders == null || !defaultRequestHeaders.Accept.Any(m =>
                    m.MediaType == "application/json"))
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(new
                        MediaTypeWithQualityHeaderValue("application/json"));
                }

                // IMPORTANT: add authorization to the header of our http client
                httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("bearer", result.AccessToken);

                // Send a GET request to the base address
                HttpResponseMessage response = await httpClient.GetAsync(config.BaseAddress);

                if (response.IsSuccessStatusCode)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    string json = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(json);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Failed to call API: {response.StatusCode}");
                    string content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Content: {content}");
                }
                Console.ResetColor();
            }
        }
    }
}