using dotenv.net;
using System;

namespace StattyBot {
    class Credentials {
        public Credentials() {      
            bool success = DotEnv.AutoConfig();

            if(success) {
                Username = Environment.GetEnvironmentVariable("STATTY_USERNAME");
                Password = Environment.GetEnvironmentVariable("STATTY_PASSWORD");
            } 
            else throw new Exception("Unable to load .env file");
        }
        public string Username;
        public string Password;
    }
}