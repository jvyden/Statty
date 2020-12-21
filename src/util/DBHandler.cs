using System;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;

namespace StattyBot {
    class DBHandler {
        SQLiteConnection connection = new SQLiteConnection("Data Source=statty.db");
        APIHandler apiHandler = new APIHandler();
        public DBHandler() {
            connection.Open();
        }

        public InternalUser getInternalUser(int id) {
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

        public bool doesUserExist(int id) {
            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "SELECT id FROM players WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SQLiteDataReader reader = command.ExecuteReader();
            return reader.Read();
        }

        public void addUser(int id) {
            Task<User> task = apiHandler.userProfile(id);
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

        public void updateUser(int id, long score, long playcount, int rank) {
            SQLiteCommand command = connection.CreateCommand();

            command.CommandText = "UPDATE players SET score = $score, playcount = $playcount, rank = $rank WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$score", score);
            command.Parameters.AddWithValue("$playcount", playcount);
            command.Parameters.AddWithValue("$rank", rank);

            command.ExecuteNonQuery();
        }
    }
}