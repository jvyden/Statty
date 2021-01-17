﻿using System;

namespace StattyBot {
    class Program {
        private static readonly Environment environment = new Environment();
        static void Main(string[] args) {
            Statty bot = new Statty(environment.Username, environment.Password, '$');
            bot.Initialize();
            Console.ReadLine();
        }
    }
}
