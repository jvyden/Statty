using System;
using System.IO;

namespace Flandre_chan_tcp {
    class Program {


        static void Main(string[] args) {
            Flandre_chan bot = new Flandre_chan(Credentials.Username, Credentials.Password, '!');
            bot.Initialize();
            Console.ReadLine();
        }
    }
}
