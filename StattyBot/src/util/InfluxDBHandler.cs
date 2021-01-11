using System;
using System.Net.Http;
using System.Threading.Tasks;


namespace StattyBot.util {
    public class InfluxDBHandler {
        private Environment environment = new Environment();
        private HttpClient client = new HttpClient();

        public InfluxDBHandler() {
            client.BaseAddress = new Uri($"http://{environment.InfluxHost}:{environment.InfluxPort}/");
            client.DefaultRequestHeaders.Accept.Clear();
        }
        
        public async Task WriteData(int playerCount, int lobbyCount, int playersInMulti, int playersInGame, int playersAfk) {
            await Task.Run(async () => {
                long time = DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1000000;
                var request = new HttpRequestMessage {
                    RequestUri = new Uri(client.BaseAddress + $"write?db=telegraf"),
                    Method = HttpMethod.Post,
                    Content = new StringContent($"statty playerCount={playerCount} {time}\n" +
                                                $"statty lobbyCount={lobbyCount} {time}\n" +
                                                $"statty playersInMulti={playersInMulti} {time}\n" +
                                                $"statty playersInGame={playersInGame} {time}\n" +
                                                $"statty playersAfk={playersAfk} {time}\n")
                };
                HttpResponseMessage response = await client.SendAsync(request);
                #if DEBUG
                if((int)response.StatusCode < 200 || (int)response.StatusCode > 299) {
                    Console.WriteLine(response.Content);
                }
                #endif
                
            });
        }

        // public void WriteData(int playerCount) {
        //     // WriteApi writeApi = influxDBClient.GetWriteApi();
        //     // writeApi.WriteRecord(WritePrecision.Ms, $"statty playerCount={playerCount}");
        // }
    }
}