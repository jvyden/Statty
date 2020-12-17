using System;
using System.IO;
using System.Threading.Tasks;

namespace Flandre_chan_tcp {
    class Program {
        static void Main(string[] args) {
            Flandre_chan bot = new Flandre_chan(Credentials.Username, Credentials.Password, '$');
            bot.Initialize();
            Console.ReadLine();

            // DBHandler dbHandler = new DBHandler();
            // Console.WriteLine(dbHandler.getInternalUser(1563) == null);
            // Console.WriteLine(dbHandler.doesUserExist(1563));
            // if(!dbHandler.doesUserExist(1563)) {
            //     dbHandler.addUser(1563);
            // }
            // APIHandler apiHandler = new APIHandler();
            // apiHandler.userProfile(1563).Wait();
        }
    }
}
