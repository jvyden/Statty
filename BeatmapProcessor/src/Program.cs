using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BeatmapProcessor {
    public class Program {
        static void Main(string[] args) {
            List<String> arguments = Regex.Matches(String.Join(" ", args), @"[\""].+?[\""]|[^ ]+")
                .Select(x => x.Value.Trim('"'))
                .ToList();

            if(arguments.Count > 0) {
                if(Directory.Exists(arguments[0])) {
                    List<String> files = new List<string>();

                    foreach(string file in Directory.GetFiles(arguments[0])) {
                        if(new DirectoryInfo(file).Extension == ".osu") files.Add(file);
                    }

                    new Processor().ParseOsuFiles(files.ToArray()).Wait();
                }
                else {
                    Console.WriteLine("Directory does not exist.");
                }
            }
            else {
                Console.WriteLine("Please specify a directory.");
            }
        }
    }
}