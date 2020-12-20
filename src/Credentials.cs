using dotenv.net;
using System;

namespace StattyBot {
    class Credentials {
        public Credentials() {      
            bool Success = DotEnv.AutoConfig();

            if(Success) {
                Username = Environment.GetEnvironmentVariable("STATTY_USERNAME");
                Password = Environment.GetEnvironmentVariable("STATTY_PASSWORD");
            } 
            else throw new Exception("Unable to load .env file");
        }
        public string Username;
        public string Password;
    }
}