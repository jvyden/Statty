using System;
using System.Collections.Generic;
using System.Linq;
using StattyBot.structs;

namespace StattyBot {
    public class PlayerList {
        private List<Player> players = new List<Player>();

        private string[] botPlayers = {"Statty", "Flandre-chan", "Beyley-chan"};

        public Player FindPlayer(string username) {
            return players.Find(player => player.Username.Equals(username));
        }
        
        public Player FindPlayer(int id) {
            return players.Find(player => player.UserID == id);
        }

        public int FindPlayerIndex(string username) {
            return players.FindIndex(player => player.Username.Equals(username));
        }
        
        public int FindPlayerIndex(int id) {
            return players.FindIndex(player => player.UserID == id);
        }

        public List<Player> GetPlayers() {
            return new List<Player>(players);
        }

        public int GetPlayersIngame() {
            int count = 0;
            foreach (Player player in players) {
                if(botPlayers.Contains(player.Username)) continue;
                if(player.Status == StatusList.PLAYING || 
                   player.Status == StatusList.MULTIPLAYING || 
                   player.Status == StatusList.PAUSED) count++;
            }
            return count;
        }
        
        public int GetPlayersAfk() {
            int count = 0;
            foreach (Player player in players) {
                if(botPlayers.Contains(player.Username)) continue;
                if(player.Status == StatusList.AFK) count++;
            }
            return count;
        }

        public void RemovePlayer(int id) {
            int index = FindPlayerIndex(id);
            if(index >= 0) {
                players.RemoveAt(index);
            }
        }

        public void RemoveAllPlayers() {
            players.Clear();
        }

        public void UpdatePlayer(Player newPlayer) {
            int index = FindPlayerIndex(newPlayer.Username);
            if(index >= 0) {
                players.RemoveAt(index);
                players.Insert(index, newPlayer);
            }
            else {
                players.Add(newPlayer);                
            }
        }
    }
}