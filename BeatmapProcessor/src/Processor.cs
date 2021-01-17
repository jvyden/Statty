using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using OppaiSharp;
using BeatmapCalc = OppaiSharp.Beatmap;

namespace BeatmapProcessor {
    public class Processor {
        private SHA256 Sha256 = SHA256.Create();
        private DbHandler dbHandler = new DbHandler();
        public async Task ParseOsuFiles(string[] files) {
            int progress = 0;
            foreach (string file in files) {
                progress++;
                StattyBeatmap beatmap = new StattyBeatmap();
                beatmap.Hash = HashFile(file);
                
                FileStream stream = File.OpenRead(file);
                StreamReader reader = new StreamReader(stream);
                
                BeatmapCalc osuFile = BeatmapCalc.Read(reader);

                DiffCalc diff = new DiffCalc().Calc(osuFile);
                beatmap.StarRating = (float) diff.Total; // no need for double precision, will be shortened to .##
                beatmap.MaxCombo = osuFile.GetMaxCombo();
                
                beatmap.ApproachRate = osuFile.AR;
                beatmap.CircleSize = osuFile.CS;
                beatmap.OverallDifficulty = osuFile.OD;
                beatmap.HpDrainRate = osuFile.HP;

                beatmap.Name = $"{osuFile.Artist} - {osuFile.Title} [{osuFile.Version}]";
                
                Console.WriteLine($"({progress}/{files.Length}) Parsed {beatmap.Name} | {beatmap.StarRating}* | AR{beatmap.ApproachRate} | CS{beatmap.CircleSize} | OD{beatmap.OverallDifficulty} | HP{beatmap.HpDrainRate}");
                dbHandler.AddBeatmap(beatmap);
            }
        }

        public string HashFile(string path) {
            using (FileStream stream = File.OpenRead(path)) {
                return BitConverter.ToString(Sha256.ComputeHash(stream)).Replace("-","").ToLower();
            }
        }
    }
}