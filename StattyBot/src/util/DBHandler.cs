using System.Data.SQLite;
using System.Threading.Tasks;
using StattyBot.structs;
using BeatmapProcessor;

namespace StattyBot.util {
    public class DbHandler {
        SQLiteConnection connection = new SQLiteConnection("Data Source=statty.db");
        ApiHandler apiHandler = new ApiHandler();
        public DbHandler() {
            connection.Open();
        }

        public InternalUser GetInternalUser(int id) {
            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT * FROM players WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SQLiteDataReader reader = command.ExecuteReader();
            if(reader.Read()) {
                long score = reader.GetInt64(1);
                long playcount = reader.GetInt64(2);
                int  rank = reader.GetInt32(3);

                return new InternalUser(id, score, playcount, rank);
            }
            else return null;
        }

        public bool DoesUserExist(int id) {
            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT id FROM players WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SQLiteDataReader reader = command.ExecuteReader();
            return reader.Read();
        }

        public void AddUser(int id) {
            Task<User> task = apiHandler.UserProfile(id);
            task.Wait();
            User user = task.Result;

            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "INSERT INTO players (id, score, playcount, rank) VALUES ($id, $score, $playcount, $rank);";
            command.Parameters.AddWithValue("$id", user.UserId);
            command.Parameters.AddWithValue("$score", 0); // Defaults are zero as to not confuse the user
            command.Parameters.AddWithValue("$playcount", 0);
            command.Parameters.AddWithValue("$rank", 0);

            command.ExecuteNonQuery();
        }

        public void UpdateUser(int id, long score, long playcount, int rank) {
            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "UPDATE players SET score = $score, playcount = $playcount, rank = $rank WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$score", score);
            command.Parameters.AddWithValue("$playcount", playcount);
            command.Parameters.AddWithValue("$rank", rank);

            command.ExecuteNonQuery();
        }

        public StattyBeatmap GetBeatmap(string hash) {
            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT * FROM beatmaps WHERE hash = $hash";
            command.Parameters.AddWithValue("$hash", hash);

            SQLiteDataReader reader = command.ExecuteReader();
            if(reader.Read()) {
                StattyBeatmap beatmap = new StattyBeatmap();
                beatmap.Hash = hash;
                beatmap.Name = reader.GetString(1);
                beatmap.CircleSize = reader.GetFloat(2);
                beatmap.ApproachRate = reader.GetFloat(3);
                beatmap.HpDrainRate = reader.GetFloat(4);
                beatmap.OverallDifficulty = reader.GetFloat(5);
                beatmap.Mode = reader.GetInt16(6);
                beatmap.TotalScore = reader.GetInt16(7);
                beatmap.MaxCombo = reader.GetInt32(8);
                beatmap.StarRating = reader.GetFloat(9);
                
                return beatmap;
            }
            return null;
        }
    }
}