using System;
using System.Collections.Generic;
using StattyBot.structs;

namespace StattyBot {
    public class PlayerList {
        private List<Player> players = new List<Player>();

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