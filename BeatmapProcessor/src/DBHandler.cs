using System.Configuration;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace BeatmapProcessor {
    public class DbHandler {
        SQLiteConnection connection = new SQLiteConnection("Data Source=statty.db");
        
        public DbHandler() {
            connection.Open();
        }

        public void AddBeatmap(StattyBeatmap beatmap) {
            SQLiteCommand command = connection.CreateCommand();
            command.CommandText =
                "INSERT INTO beatmaps (hash, name, circlesize, approachrate, hpdrainrate, overalldifficulty, maxcombo, starrating) VALUES ($hash, $name, $circlesize, $approachrate, $hpdrainrate, $overalldifficulty, $maxcombo, $starrating);";

            command.Parameters.AddWithValue("$hash", beatmap.Hash);
            command.Parameters.AddWithValue("$name", beatmap.Name);
            command.Parameters.AddWithValue("$circlesize", beatmap.CircleSize);
            command.Parameters.AddWithValue("$approachrate", beatmap.ApproachRate);
            command.Parameters.AddWithValue("$hpdrainrate", beatmap.HpDrainRate);
            command.Parameters.AddWithValue("$overalldifficulty", beatmap.OverallDifficulty);
            command.Parameters.AddWithValue("$maxcombo", beatmap.MaxCombo);
            command.Parameters.AddWithValue("$starrating", beatmap.StarRating);

            command.ExecuteNonQuery();
        }
    }
}