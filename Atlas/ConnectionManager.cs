using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Atlas
{
    internal class ConnectionManager
    {
        private String serverAddress = null;

        public ConnectionManager(Settings settings)
        {
            serverAddress = settings.serverAddress;
        }

        public async Task DoGetRequest()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var url = "https://example.com/api/data";

            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                var json = JsonSerializer.Deserialize<JsonElement>(responseBody);
                Console.WriteLine(json);
            }
            else
            {
                Console.WriteLine($"Failed with status code {response.StatusCode}");
            }
        }
    }
}