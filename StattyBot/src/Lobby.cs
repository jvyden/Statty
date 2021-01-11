using System.Collections.Generic;
using StattyBot.structs;

namespace StattyBot {
    public class Lobby {
        private List<MultiplayerRoom> rooms = new List<MultiplayerRoom>();
        
        public MultiplayerRoom FindRoom(int id) {
            return rooms.Find(room => room.MatchId == id);
        }
        
        public int FindRoomIndex(int id) {
            return rooms.FindIndex(room => room.MatchId == id);
        }

        public void UpdateRoom(MultiplayerRoom newRoom) {
            int index = FindRoomIndex(newRoom.MatchId);
            if(index >= 0) {
                rooms.RemoveAt(index);
                rooms.Insert(index, newRoom);
            }
            else {
                rooms.Add(newRoom);
            }
        }

        public List<MultiplayerRoom> GetRooms() {
            return new List<MultiplayerRoom>(rooms);
        }

        public void RemoveAllRooms() {
            rooms.Clear();
        }

        public void RemoveRoom(int id) {
            int index = FindRoomIndex(id);
            if(index >= 0) {
                rooms.RemoveAt(index);
            }
        }
        
        public int GetPlayerCount() {
            int count = 0;
            foreach (MultiplayerRoom room in rooms) {
                count += room.GetPlayerCount();
            }
            return count;
        }
    }
}