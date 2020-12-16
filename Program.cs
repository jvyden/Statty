using System;
using System.IO;

namespace Flandre_chan_tcp {
    class Program {


        static void Main(string[] args) {
            Flandre_chan bot = new Flandre_chan("Flandre-chan", "!Pikusia24", '!');
            bot.Initialize();
            Console.ReadLine();
        }
    }
}
