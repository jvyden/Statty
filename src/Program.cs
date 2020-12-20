using System;
using System.IO;
using System.Threading.Tasks;

namespace StattyBot {
    class Program {
        private static Credentials credentials = new Credentials();
        static void Main(string[] args) {
            Statty bot = new Statty(credentials.Username, credentials.Password, '$');
            bot.Initialize();
            Console.ReadLine();
        }
    }
}
