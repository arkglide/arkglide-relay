using System;
using System.Threading.Tasks;

namespace arkglide_relay
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("ArkGlide Relay v1.5 启动中...");

            // 1. 初始化全局配置
            var serverList = new ServerList();

            // 2. 启动 HTTP Index Server (8081)
            var http = new WebHttp(serverList);
            serverList._webHttp = http;
            http.Start(8081);

            // 3. 初始化并启动 WebSocket Relay (8080)
            var relay = new ServerEnity();
            relay.Execute.Post(); // 加载 script.js
            serverList.AddServerEnity(relay);

            var ws = new _WebSocket(serverList);
            relay.SocketSvr = ws;
            ws.SetListenUrl("http://0.0.0.0:8080").EngineInstall();
            _ = ws.StartServerEngine();

            Console.WriteLine("服务端已全部就绪！");
            Console.WriteLine(" - HTTP 页面: http://localhost:8081");
            Console.WriteLine(" - WS 接口: ws://localhost:8080/ws_game");
            Console.WriteLine("按回车键退出...");

            Console.ReadLine();
        }
    }
}