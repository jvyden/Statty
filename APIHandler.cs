using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;



namespace Flandre_chan_tcp {
    class APIHandler {
        private HttpClient client = new HttpClient();
        public APIHandler() {
            client.BaseAddress = new Uri("https://oldsu.ayyeve.xyz/api/");
            client.DefaultRequestHeaders.Accept.Clear();
        }

        public async Task<User> userProfile(int id) {
            return await Task.Run(async () => {
                var request = new HttpRequestMessage {
                    RequestUri = new Uri(client.BaseAddress + "userProfile"),
                    Method = HttpMethod.Get,
                    Headers = {{ "userId", id.ToString() }}
                };

                HttpResponseMessage response = (await client.SendAsync(request));
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);

                return JsonConvert.DeserializeObject<User>(content);
            });
        }

        public async Task<User> userProfile(string username) {
            return await Task.Run(async () => {
                var request = new HttpRequestMessage {
                    RequestUri = new Uri(client.BaseAddress + "userProfile"),
                    Method = HttpMethod.Get,
                    Headers = {{"username", username}}
                };

                HttpResponseMessage response = (await client.SendAsync(request));
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(content);

                return JsonConvert.DeserializeObject<User>(content);
            });
        }
    }
}