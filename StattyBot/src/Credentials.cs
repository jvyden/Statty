using dotenv.net;
using System;

namespace StattyBot {
    class Credentials {
        public Credentials() {      
            bool success = DotEnv.AutoConfig();

            if(success) {
                Username = Environment.GetEnvironmentVariable("STATTY_USERNAME");
                Password = Environment.GetEnvironmentVariable("STATTY_PASSWORD");
                Host = Environment.GetEnvironmentVariable("STATTY_HOST");
                Port = Int32.Parse(Environment.GetEnvironmentVariable("STATTY_PORT") ?? "-1");
            } 
            else throw new Exception("Unable to load .env file");
        }
        public string Username;
        public string Password;
        public string Host;
        public int Port;
    }
}