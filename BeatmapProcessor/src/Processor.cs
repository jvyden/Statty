using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using OppaiSharp;
using OsuParsers.Decoders;
using Beatmap = OsuParsers.Beatmaps.Beatmap;
using BeatmapCalc = OppaiSharp.Beatmap;

namespace BeatmapProcessor {
    public class Processor {
        private SHA256 Sha256 = SHA256.Create();
        private DbHandler dbHandler = new DbHandler();
        public async Task ParseOsuFiles(string[] files) {
            foreach (string file in files) {
                StattyBeatmap beatmap = new StattyBeatmap();
                beatmap.Hash = HashFile(file);

                Beatmap osuFile = BeatmapDecoder.Decode(file);
                
                beatmap.ApproachRate = osuFile.DifficultySection.ApproachRate;
                beatmap.CircleSize = osuFile.DifficultySection.CircleSize;
                beatmap.OverallDifficulty = osuFile.DifficultySection.OverallDifficulty;
                beatmap.HpDrainRate = osuFile.DifficultySection.HPDrainRate;
                
                FileStream stream = File.OpenRead(file);
                StreamReader reader = new StreamReader(stream);
                
                BeatmapCalc beatmapCalc = BeatmapCalc.Read(reader);

                DiffCalc diff = new DiffCalc().Calc(beatmapCalc);
                beatmap.StarRating = (float) diff.Total;
                beatmap.MaxCombo = beatmapCalc.GetMaxCombo();

                beatmap.Name = $"{osuFile.MetadataSection.Artist} - {osuFile.MetadataSection.Title} [{osuFile.MetadataSection.Version}]";
                
                // Console.WriteLine($"{beatmap.Name} | {beatmap.StarRating}* | AR{beatmap.ApproachRate} | CS{beatmap.CircleSize} | OD{beatmap.OverallDifficulty} | HP{beatmap.HpDrainRate}");
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