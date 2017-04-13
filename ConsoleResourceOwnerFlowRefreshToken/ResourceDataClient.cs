using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleResourceOwnerFlowRefreshToken
{
    public static class ResourceDataClient
    {
        public static async Task GetDataAndDisplayInConsoleAsync(string access_token)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.SetBearerToken(access_token);

            var payloadFromResourceServer = await httpClient.GetAsync("https://localhost:44365/api/DataEventRecords");
            if (!payloadFromResourceServer.IsSuccessStatusCode)
            {
                Console.WriteLine(payloadFromResourceServer.StatusCode);
            }
            else
            {
                var content = await payloadFromResourceServer.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }

        }
    }
}
