using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkglide_relay
{
    public class ServerEnity{
        public JSExecute Execute {  get; set; }
        public List<Rooms> TRooms { get; set; }
        public _WebSocket SocketSvr {get; set;}
    }
    public class ServerList
    {
        private List<ServerEnity> _serverEnities = new List<ServerEnity>();

        public ServerList AddServerEnity(ServerEnity serverEnity)
        {
            _serverEnities.Add(serverEnity);
            return this;
        }
    }
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<Player> Players = new List<Player>();
        public List<string>? Ident {  get; set; }
        public int? MaxPlayers { get; set; }
        public int Level { get; set; }
        public List<string>? Data { get; set; }
        public JSValue jsValue { get; set; }
        private Rooms _Rooms;
        public Room(Rooms rooms) { _Rooms = rooms; }
        public Rooms GetRooms()
        { return _Rooms; }
        public Room Accept(Player _player)
        {
            if (MaxPlayers != null&&Players.Count >= MaxPlayers) {
                return GetRooms().CreateRoom().Accept(_player);
            }
            return this;
        }
    }
    public class Player
    {
        public int NodeID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<string>? Data { get; set; }
    }
}
