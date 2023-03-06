using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;

using Newtonsoft.Json.Linq;

namespace Atlas
{
    internal class ConnectionManager
    {
        private ConnectionSettings settings = null;

        public ConnectionManager(ConnectionSettings pSettings)
        {
            settings = pSettings;
        }

        public async Task<int> SendPostRequestAsync(String endPoint, Dictionary<String, String> parameters)
        {
            try
            {
                Uri uriResult;
                bool isUrlValid = Uri.TryCreate(settings.serverAddress, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (!isUrlValid)
                {
                    Debug.WriteLine($"[*] ({this.GetType().Name}) Server address is not a valid URL: {settings.serverAddress}");
                    throw new Exception($"Server address is not a valid URL: {settings.serverAddress}");
                }

                string url = $"{settings.serverAddress}{endPoint}";

                using (var httpClient = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(parameters);
                    var response = await httpClient.PostAsync(url, content);

                    response.EnsureSuccessStatusCode();

                    var responseContent = await response.Content.ReadAsStringAsync();

                    var jsonResponse = JObject.Parse(responseContent);
                    var cmd = (int)jsonResponse["cmd"];

                    if (!Enum.IsDefined(typeof(ConnectionSettings.vaildResponces), cmd))
                    {
                        Debug.WriteLine($"[*] ({this.GetType().Name}) Server Response did not contain a valid 'cmd' field.");
                        throw new Exception("Server Response did not contain a valid 'cmd' field.");
                    }

                    Debug.WriteLine($"[*] ({this.GetType().Name}) Server Response: {cmd}");
                    return cmd;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[*] ({this.GetType().Name}) Error: {ex.Message}");
                throw;
            }
        }
    }
}