using System;
using System.Collections.Generic;

namespace arkglide_relay
{
    public class ServerEnity
    {
        public string ServerId { get; set; } = "arkglide-official-01"; // 新增
        public JSExecute Execute { get; set; } = new();
        public List<Rooms> TRooms { get; set; } = new();
        public _WebSocket SocketSvr { get; set; }
    }

    public class ServerList
    {
        public List<ServerEnity> _serverEnities = new();
        public WebHttp _webHttp { get; set; } // 去掉 new()，由 Program 注入
        public ServerEnity MainRelay { get; set; }

        public ServerList AddServerEnity(ServerEnity serverEnity)
        {
            _serverEnities.Add(serverEnity);
            if (MainRelay == null) MainRelay = serverEnity;
            return this;
        }
    }

    public class Rooms // 相当于 RoomGroup
    {
        private List<Room> _Rooms = new();
        public _WebSocket GetSocket { get; set; }

        public Room CreateRoom(string roomId = "default")
        {
            var room = new Room(this) { RoomId = roomId };
            _Rooms.Add(room);
            return room;
        }
    }

    public class Room
    {
        public string RoomId { get; set; } // 改为 string
        public string Name { get; set; }
        public List<Player> Players { get; set; } = new();
        public int? MaxPlayers { get; set; } = 8;
        public List<string>? Data { get; set; }

        private Rooms _Rooms;

        public Room(Rooms rooms) { _Rooms = rooms; }

        public Rooms GetRooms() => _Rooms;

        public Room Accept(Player player)
        {
            if (MaxPlayers != null && Players.Count >= MaxPlayers)
            {
                var newRoom = GetRooms().CreateRoom($"{RoomId}_2");
                return newRoom.Accept(player);
            }
            Players.Add(player); // 修复：一定要把玩家加进去
            return this;
        }
    }

    public class Player
    {
        public string NodeID { get; set; } // 改为 string
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<string>? Data { get; set; }
    }
}