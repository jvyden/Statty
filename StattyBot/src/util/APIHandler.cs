using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StattyBot.structs;

namespace StattyBot.util {
    public class ApiHandler {
        private HttpClient client = new HttpClient();
        public ApiHandler() {
            client.BaseAddress = new Uri("https://oldsu.ayyeve.xyz/api/");
            client.DefaultRequestHeaders.Accept.Clear();
        }

        public async Task<User> UserProfile(int id) {
            return await Task.Run(async () => {
                HttpRequestMessage request = new HttpRequestMessage {
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

        public async Task<User> UserProfile(string username) {
            #if DEBUG
            Console.WriteLine("Attempting to find ID of " + username);
            #endif
            return await Task.Run(async () => {
                HttpRequestMessage request = new HttpRequestMessage {
                    RequestUri = new Uri(client.BaseAddress + "userProfile"),
                    Method = HttpMethod.Get,
                    Headers = {{"username", username}}
                };

                HttpResponseMessage response = (await client.SendAsync(request));
                string content = await response.Content.ReadAsStringAsync();
                
                #if DEBUG
                Console.WriteLine(content);
                #endif

                return JsonConvert.DeserializeObject<User>(content);
            });
        }
    }
}