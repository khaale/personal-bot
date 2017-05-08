using Messages.Model.Torrents;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Messages.Services.Torrents
{
    public class RutrackerService
    {
        private static readonly string ApiBaseUrl = "http://api.rutracker.org/v1/";
        private readonly HttpClient _client;

        public RutrackerService()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            _client = new HttpClient(handler);
            _client.BaseAddress = new Uri(ApiBaseUrl);
        }

        public async Task<IReadOnlyCollection<Topic>> GetTopicsAsync(params string[] ids)
        {
            var idsEncoded = string.Join("%2C", ids);
            var url = $"get_tor_topic_data?by=topic_id&val={idsEncoded}";

            var httpResponse = await _client.GetAsync(url);
            httpResponse.EnsureSuccessStatusCode();
            var rawResponse = await httpResponse.Content.ReadAsStringAsync();

            JObject response = JsonConvert.DeserializeObject<JObject>(rawResponse);

            var result = 
                from topicProperty in response.SelectToken("result").Children<JProperty>()
                let topicData = topicProperty.Value
                where topicData.HasValues
                select new Topic(
                        topicProperty.Name,
                        topicData.SelectToken("info_hash").Value<string>(),
                        topicData.SelectToken("reg_time").Value<int>(),
                        topicData.SelectToken("topic_title").Value<string>()
                        );

            return result.ToList();
        }
    }
}
