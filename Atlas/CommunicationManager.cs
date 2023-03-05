using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;

namespace Atlas.Atlas
{
    internal class CommunicationManager
    {
        private Settings settings = null;

        public CommunicationManager(Settings pSettings)
        {
            settings = pSettings;
        }

        public async Task DoPostRequestAsync(String endPoint)
        {
            String url = settings.serverAddress + endPoint;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var parameters = new NameValueCollection();
            parameters.Add("machine_id", "value1");

            var data = Encoding.UTF8.GetBytes(BuildQueryString(parameters));

            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            var response = (HttpWebResponse)await request.GetResponseAsync();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var result = await streamReader.ReadToEndAsync();
                JObject keyValuePairs = JObject.Parse(result);

                Debug.WriteLine($"[*] (CommunicationManager) Server Reseponce: {keyValuePairs["cmd"]}");
            }
        }

        private static string BuildQueryString(NameValueCollection parameters)
        {
            var queryString = new StringBuilder();
            foreach (string key in parameters.Keys)
            {
                queryString.AppendFormat("{0}={1}&", WebUtility.UrlEncode(key), WebUtility.UrlEncode(parameters[key]));
            }
            queryString.Length--; // Remove the trailing '&'
            return queryString.ToString();
        }
    }
}