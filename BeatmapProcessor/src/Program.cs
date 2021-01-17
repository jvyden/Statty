using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
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

                    Console.WriteLine($"Parsing {files.Count} osu files...");

                    int coreCount = Environment.ProcessorCount; // Sometimes inaccurate on multi-cpu systems, but I don't care.
                    List<String[]> chunkedFiles = GetChunkedArrays(files.ToArray(), coreCount);
                    
                    Thread[] threads = new Thread[coreCount];
                    for (int i = 0; i < coreCount; i++) { // Initialize threads
                        int localIteration = i; // prevents crossing over
                        threads[i] = new Thread(() => {
                            new Processor().ParseOsuFiles(chunkedFiles[localIteration], localIteration).Wait();
                        });
                    }
                    
                    for (int i = 0; i < coreCount; i++) { // Start threads
                        threads[i].Start();
                    }
                    
                    for (int i = 0; i < coreCount; i++) {  // Wait for all threads to stop
                        threads[i].Join();
                    }
                    new DbHandler().ProcessQueue();
                }
                else {
                    Console.WriteLine("Directory does not exist.");
                }
            }
            else {
                Console.WriteLine("Please specify a directory.");
            }
        }

        private static List<String[]> GetChunkedArrays(string[] mainArray, int chunkFactor) {
            int chunkSize = mainArray.Length / chunkFactor;
            List<String[]> chunkedArray = new List<string[]>();
            
            for (int i = 0; i < mainArray.Length; i += chunkSize) {
                string[] buffer = new string[chunkSize];
                int length = chunkSize;
                
                if(mainArray.Length - i < chunkSize) {
                    length = mainArray.Length - i; // Only copy remainder
                }
                Array.Copy(mainArray, i, buffer, 0, length);
                chunkedArray.Add(buffer);
            }
            
            return chunkedArray;
        }
    }
}