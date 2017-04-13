using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleResourceOwnerFlowRefreshToken
{
    public class Program
    {
        static TokenClient _tokenClient;

        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        static async Task MainAsync()
        {
            Console.Title = "Console ResourceOwner Flow RefreshToken";

            var disco = await DiscoveryClient.GetAsync("https://localhost:44318");
            if (disco.IsError) throw new Exception(disco.Error);

            _tokenClient = new TokenClient(
                disco.TokenEndpoint,
                "resourceownerclient",
                "dataEventRecordsSecret");

            var response = await RequestTokenAsync();
            //response.Show();

            Console.ReadLine();

            var refresh_token = response.RefreshToken;

            while (true)
            {
                response = await RefreshTokenAsync(refresh_token);
                ShowResponse(response);

                Console.ReadLine();
                await CallServiceAsync(response.AccessToken);

                if (response.RefreshToken != refresh_token)
                {
                    refresh_token = response.RefreshToken;
                }
            }
        }

        static async Task<TokenResponse> RequestTokenAsync()
        {
            return await _tokenClient.RequestResourceOwnerPasswordAsync(
                "bob",
                "bob",
                "email openid dataEventRecords offline_access");
        }

        private static async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            Console.WriteLine("Using refresh token: {0}", refreshToken);

            return await _tokenClient.RequestRefreshTokenAsync(refreshToken);
        }

        static async Task CallServiceAsync(string token)
        {
            var baseAddress = "https://localhost:44318/";

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.SetBearerToken(token);
            var response = await client.GetStringAsync("identity");

            Console.WriteLine("\n\nService claims:");
            Console.WriteLine(JArray.Parse(response));
        }

        private static void ShowResponse(TokenResponse response)
        {
            if (!response.IsError)
            {
                Console.WriteLine("Token response:");
                Console.WriteLine(response.Json);

                if (response.AccessToken.Contains("."))
                {
                    Console.WriteLine("\nAccess Token (decoded):");

                    var parts = response.AccessToken.Split('.');
                    var header = parts[0];
                    var claims = parts[1];

                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(header))));
                    Console.WriteLine(JObject.Parse(Encoding.UTF8.GetString(Base64Url.Decode(claims))));
                }
            }
            else
            {
                if (response.ErrorType == ResponseErrorType.Http)
                {
                    Console.WriteLine("HTTP error: ");
                    Console.WriteLine(response.Error);
                    Console.WriteLine("HTTP status code: ");
                    Console.WriteLine(response.HttpStatusCode);
                }
                else
                {
                    Console.WriteLine("Protocol error response:");
                    Console.WriteLine(response.Json);
                }
            }
        }
    }
}