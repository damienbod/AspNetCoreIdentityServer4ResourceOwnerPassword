
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleResourceOwnerFlowRefreshToken
{
    public static class IdentityServer4Client
    {
        private static TokenClient _tokenClient;

        public static async Task<TokenResponse> LoginAsync(string user, string password)
        {
            Console.Title = "Console ResourceOwner Flow RefreshToken";

            var disco = await DiscoveryClient.GetAsync("https://localhost:44318");
            if (disco.IsError) throw new Exception(disco.Error);

            _tokenClient = new TokenClient(
                disco.TokenEndpoint,
                "resourceownerclient",
                "dataEventRecordsSecret");

            return await RequestTokenAsync(user, password);
        }

        public static async Task RunRefreshAsync(TokenResponse response)
        {
            var refresh_token = response.RefreshToken;

            while (true)
            {
                response = await RefreshTokenAsync(refresh_token);
                ShowResponse(response);

                await CallServiceAsync(response.AccessToken);

                if (response.RefreshToken != refresh_token)
                {
                    refresh_token = response.RefreshToken;
                }
            }
        }
        private static async Task<TokenResponse> RequestTokenAsync(string user, string password)
        {
            return await _tokenClient.RequestResourceOwnerPasswordAsync(
                user,
                password,
                "email openid dataEventRecords offline_access");
        }

        private static async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
        {
            Console.WriteLine("Using refresh token: {0}", refreshToken);

            return await _tokenClient.RequestRefreshTokenAsync(refreshToken);
        }

        private static async Task CallServiceAsync(string token)
        {
            var baseAddress = "https://localhost:44318/";

            var client = new HttpClient
            {
                BaseAddress = new Uri(baseAddress)
            };

            client.SetBearerToken(token);
            var response = await client.GetStringAsync("https://localhost:44318/identity");

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
