using System;
using System.IO;
using System.Threading.Tasks;

namespace StattyBot {
    class Program {
        static void Main(string[] args) {
            Statty bot = new Statty(Credentials.Username, Credentials.Password, '$');
            bot.Initialize();
            Console.ReadLine();
        }
    }
}
