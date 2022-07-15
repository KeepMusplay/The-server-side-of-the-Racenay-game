using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerRacenay
{
    class Room
    {
        public int idRoom;
        public Player[] players; // [0] - own room
        public bool locked;


        int map; // id map
        Race rice;



        // launch rice
        public void StartRice() => (rice = new Race()).Init(this);


        // destroy this room
        public void DeleteRoom()
        {
            Server.rooms[idRoom] = null;

            foreach (Player p in players)
                p.SetActiveRoom(null); // throw every player in garage
        }


        public Room(int _idRoom, int _maxCountParts, Player player, int _map)
        {
            idRoom = _idRoom;
            players = new Player[_maxCountParts];
            players[0] = player;
            map = _map;
            locked = false;
        }
    }
}
