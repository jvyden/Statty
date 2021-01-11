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
        
        public async Task WriteData(int playerCount) {
            await Task.Run(async () => {
                var request = new HttpRequestMessage {
                    RequestUri = new Uri(client.BaseAddress + $"write?db=telegraf"),
                    Method = HttpMethod.Post,
                    Content = new StringContent($"statty playerCount={playerCount} {DateTimeOffset.Now.ToUnixTimeMilliseconds() * 1000000}")
                };
                HttpResponseMessage response = await client.SendAsync(request);
            });
        }

        // public void WriteData(int playerCount) {
        //     // WriteApi writeApi = influxDBClient.GetWriteApi();
        //     // writeApi.WriteRecord(WritePrecision.Ms, $"statty playerCount={playerCount}");
        // }
    }
}