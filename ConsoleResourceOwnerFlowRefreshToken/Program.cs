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
        
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        static async Task MainAsync()
        {
            var response = IdentityServer4Client.LoginAsync("damienbod", "damienbod").Result;
  
            Console.WriteLine($"GOT TOKENS FROM IDENTITYSERVER4: {response.AccessToken}");

            // GET DATA from the resource server
            await ResourceDataClient.GetDataAndDisplayInConsoleAsync(response.AccessToken);

            Console.WriteLine($"GOT DATA FROM THE RESOURCE SERVER");

            // Run an loop which gets refreshes the token every 3000 milliseconds
            await IdentityServer4Client.RunRefreshAsync(response, 3000);
        }
    }
}