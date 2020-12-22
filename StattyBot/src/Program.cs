using System;

namespace StattyBot {
    class Program {
        private static Credentials _credentials = new Credentials();
        static void Main(string[] args) {
            Statty bot = new Statty(_credentials.Username, _credentials.Password, '$');
            bot.Initialize();
            Console.ReadLine();
        }
    }
}
