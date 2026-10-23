using System;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace arkglide_relay
{
    public class _WebSocket : IAsyncDisposable
    {
        private WebApplication _ServerSupportEngine;
        private WebApplicationBuilder _ServerBuilder;
        private string? _ServerUrl;
        private ServerList _serverList;

        public _WebSocket(ServerList serverList) => _serverList = serverList;

        public _WebSocket SetListenUrl(string url) { _ServerUrl = url; return this; }

        public _WebSocket EngineInstall()
        {
            _ServerBuilder = WebApplication.CreateBuilder();
            _ServerSupportEngine = _ServerBuilder.Build();
            _ServerSupportEngine.UseWebSockets();

            _ServerSupportEngine.Map("/ws_game", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await HandleWebSocket(webSocket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            });
            return this;
        }

        private async Task HandleWebSocket(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 64];
            long clientSeq = 0;

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close) break;

                var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var relay = _serverList.MainRelay;

                try
                {
                    var root = JsonNode.Parse(msg);
                    var type = root?["type"]?.ToString();
                    var reqData = root?["data"];

                    // 1. 构造信封
                    var env = new JSValue.Envelope<object>
                    {
                        type = type,
                        seq = ++clientSeq,
                        ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                        ident = $"{relay.ServerId}|-",
                        data = reqData
                    };

                    // 2. 路由处理 (比如收到 auth 或 game_message)
                    if (type == "game_message")
                    {
                        var eventName = reqData?["event"]?.ToString();
                        var payloadStr = reqData?["payload"]?.ToJsonString();

                        // 调用 JS 钩子
                        var jsReturn = relay.Execute.Dispatch("OnClientGameMessage", "p_1", "room_1", eventName, payloadStr);

                        // 构造 game_message 回包（这里为了测试，直接发回给客户端）
                        var gameData = new JSValue.GameMessageData
                        {
                            fromPlayerId = "p_1",
                            @event = eventName ?? "unknown",
                            payload = JsonSerializer.Deserialize<object>(payloadStr ?? "{}")
                        };

                        var packet = new JSValue.Envelope<JSValue.GameMessageData>
                        {
                            type = "game_message",
                            seq = ++clientSeq,
                            ts = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                            ident = $"{relay.ServerId}|room_1",
                            data = gameData
                        };

                        var json = JsonSerializer.Serialize(packet);
                        var bytes = Encoding.UTF8.GetBytes(json);
                        await webSocket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[WS Error] {ex.Message}");
                }
            }
        }

        public async Task StartServerEngine()
            => await _ServerSupportEngine.RunAsync(_ServerUrl ?? throw new Exception("E-Server:unknown url"));

        public async ValueTask DisposeAsync()
        {
            if (_ServerSupportEngine != null) await _ServerSupportEngine.DisposeAsync();
        }
    }
}