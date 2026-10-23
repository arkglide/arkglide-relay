using System.Collections.Generic;

namespace arkglide_relay
{
    /// <summary>
    /// 协议数据模型容器（DTOs）
    /// 仅定义数据结构，不参与任何序列化、网络发送或 Seq 生成
    /// </summary>
    public static class JSValue
    {
        // ==========================================
        // 基础信封（如果不想用泛型，也可以定义这个）
        // ==========================================
        public class Envelope<T>
        {
            public string type { get; set; } = "";
            public long seq { get; set; }
            public long ts { get; set; }
            public string ident { get; set; } = "";
            public T data { get; set; }
        }

        // ==========================================
        // 以下是具体的业务消息类
        // ==========================================

        public class AuthData
        {
            public string playerName { get; set; } = "";
            public string protocolVersion { get; set; } = "1.5";
            public string clientVersion { get; set; } = "0.1.5";
            public string preferredCodec { get; set; } = "empty";
        }

        public class AuthOkData
        {
            public string playerId { get; set; } = "";
            public string sessionToken { get; set; } = "";
            public ServerInfo server { get; set; } = new();
        }

        public class ServerInfo
        {
            public string serverId { get; set; } = "";
            public string serverName { get; set; } = "";
            public string protocolVersion { get; set; } = "1.5";
            public string mode { get; set; } = "ce_";
            // ... 其他签名字段
        }

        public class JoinRoomData
        {
            public string groupId { get; set; } = "default";
            public string roomId { get; set; } = "";
            public string playerName { get; set; } = "";
        }

        public class RoomJoinedData
        {
            public string groupId { get; set; } = "";
            public string roomId { get; set; } = "";
            public string playerId { get; set; } = "";
            public List<PlayerInfo> players { get; set; } = new();
        }

        public class PlayerInfo
        {
            public string playerId { get; set; } = "";
            public string playerName { get; set; } = "";
        }

        public class GameMessageData
        {
            public string fromPlayerId { get; set; }
            public string @event { get; set; } = "";
            public object payload { get; set; }
        }

        public class ErrorData
        {
            public string code { get; set; } = "";
            public string message { get; set; } = "";
            public long? refSeq { get; set; }
        }
    }
}