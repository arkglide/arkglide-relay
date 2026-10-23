using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace arkglide_relay
{
    public class WebHttp
    {
        private HttpListener? _listener;
        private CancellationTokenSource? _cts;

        // 你这里的列表数据从哪里来？可以注入 ServerList 或者维护一个静态表
        private ServerList _serverList;

        public WebHttp(ServerList serverList)
        {
            _serverList = serverList;
        }

        public void Start(int port)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://*:{port}/");
            _listener.Start();
            _cts = new CancellationTokenSource();

            _ = Task.Run(() => ListenLoopAsync(_cts.Token));
            Console.WriteLine($"[IndexServer] HTTP 索引服务已启动: http://localhost:{port}/");
        }

        private async Task ListenLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested && _listener!.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = Task.Run(() => HandleRequestAsync(context));
                }
                catch (Exception) when (token.IsCancellationRequested) { break; }
                catch (Exception ex) { Console.WriteLine($"[IndexServer Error] {ex.Message}"); }
            }
        }
        private string GetHtmlPage()
        {
            return """
<!DOCTYPE html>
<html lang="zh-CN">
<head>
    <meta charset="UTF-8">
    <title>ArkGlide 服务器列表</title>
    <style>
        body { font-family: sans-serif; margin: 20px; color: #000; }
        table { border-collapse: collapse; width: 100%; max-width: 800px; }
        th, td { border: 1px solid #000; padding: 4px 8px; text-align: left; font-size: 14px; }
        th { background-color: #f0f0f0; font-weight: bold; text-align: center; }
        a { color: #0000ee; text-decoration: underline; cursor: pointer; }
        .status-online { color: green; }
        .status-offline { color: red; }
        .btn-copy { background: #e0e0e0; border: 1px solid #000; padding: 2px 6px; cursor: pointer; font-size: 12px; }
    </style>
</head>
<body>
    <table>
        <thead>
            <tr><th>名称</th><th>地址</th><th>状态</th><th>操作</th></tr>
        </thead>
        <tbody id="server-list"><tr><td colspan="4" style="text-align: center;">加载中...</td></tr></tbody>
    </table>
    <script>
        window.onload = async () => {
            try {
                const res = await fetch('/api/v1/servers');
                const data = await res.json();
                renderServers(data.servers);
            } catch (e) {
                document.getElementById('server-list').innerHTML = '<tr><td colspan="4">加载失败</td></tr>';
            }
        };
        function renderServers(servers) {
            const tbody = document.getElementById('server-list');
            tbody.innerHTML = '';
            servers.forEach(s => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td><a href="#" onclick='connect("${s.address}")'>${s.serverName}</a></td>
                    <td>${s.address}</td>
                    <td class='${s.online ? 'status-online' : 'status-offline'}'>${s.online ? '在线' : '离线'}</td>
                    <td><button class='btn-copy' onclick='copyAddr("${s.address}", this)'>复制地址</button></td>
                `;
                tbody.appendChild(tr);
            });
        }
        function connect(addr) { alert('准备连接到: ' + addr); }
        function copyAddr(text, btn) {
            navigator.clipboard.writeText(text).then(() => {
                btn.innerText = '已复制!';
                setTimeout(() => btn.innerText = '复制地址', 1500);
            });
        }
    </script>
</body>
</html>
""";
        }
        private async Task HandleRequestAsync(HttpListenerContext context)
        {
            var req = context.Request;
            var res = context.Response;
            var path = req.Url?.AbsolutePath ?? "/";

            try
            {
                if (req.HttpMethod == "GET" && (path == "/" || path == "/servers"))
                {
                    // 返回 HTML
                    string html =GetHtmlPage(); // 这里放之前讨论的 HTML
                    await WriteResponseAsync(res, 200, html, "text/html; charset=utf-8");
                }
                else if (req.HttpMethod == "GET" && path == "/api/v1/servers")
                {
                    // 返回 JSON 列表
                    // 这里可以和 _serverList 联动，获取所有 ServerEnity 的地址和人数
                    string json = "{\"servers\":[]}";
                    await WriteResponseAsync(res, 200, json, "application/json; charset=utf-8");
                }
                else
                {
                    await WriteResponseAsync(res, 404, "Not Found", "text/plain");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[IndexServer Error] {ex.Message}");
                try { res.Abort(); } catch { }
            }
            finally
            {
                res.Close();
            }
        }

        private async Task WriteResponseAsync(HttpListenerResponse response, int statusCode, string content, string contentType)
        {
            response.StatusCode = statusCode;
            response.ContentType = contentType;
            response.Headers.Add("Access-Control-Allow-Origin", "*");
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
    }
}