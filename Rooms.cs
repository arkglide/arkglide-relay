using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkglide_relay
{
    public class Rooms
    {
        private List<Room> _Rooms=new List<Room>();
        public _WebSocket GetSocket {  get; set; }
        public Room CreateRoom()
        {
            var room=new Room(this);
            _Rooms.Add(room);
            return room;
        }
    }
}
