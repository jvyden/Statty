using dotenv.net;
using System;

namespace StattyBot {
    class Environment {
        public string Username;
        public string Password;
        public string Host;
        public int Port;
        public bool InfluxEnabled;
        public string InfluxHost;
        public int InfluxPort;
        public Environment() {      
            bool success = DotEnv.AutoConfig();

            if(success) {
                Username = System.Environment.GetEnvironmentVariable("STATTY_USERNAME");
                Password = System.Environment.GetEnvironmentVariable("STATTY_PASSWORD");
                Host = System.Environment.GetEnvironmentVariable("STATTY_HOST");
                Port = Int32.Parse(System.Environment.GetEnvironmentVariable("STATTY_PORT") ?? "-1");
                
                InfluxEnabled = System.Environment.GetEnvironmentVariable("STATTY_INFLUX_ENABLED") == "true";
                InfluxHost = System.Environment.GetEnvironmentVariable("STATTY_INFLUX_HOST");
                InfluxPort = Int32.Parse(System.Environment.GetEnvironmentVariable("STATTY_INFLUX_PORT") ?? "-1");
            } 
            else throw new Exception("Unable to load .env file");
        }
    }
}