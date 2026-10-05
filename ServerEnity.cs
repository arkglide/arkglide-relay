using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace arkglide_relay
{
    public class ServerEnity{ }
    public class Room
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<Player> Players { get; set; }
        public List<string>? Ident {  get; set; }
        public int Level { get; set; }
        public List<string>? Data { get; set; }
    }
    public class Player
    {
        public int NodeID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public List<string>? Data { get; set; }
    }
}
