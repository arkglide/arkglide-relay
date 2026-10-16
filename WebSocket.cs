using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore;

namespace arkglide_relay
{
    public class _WebSocket:IAsyncDisposable
    {
        private WebApplication _ServerSupportEngine;
        private WebApplicationBuilder _ServerBuilder;
        private string? _ServerUrl;
        public _WebSocket SetListenUrl(string url)
        {
            _ServerUrl = url??null;
            return this;
        }
        public _WebSocket EngineUpdater(WebApplicationBuilder builder)
        {
            _ServerSupportEngine = builder.Build();
            _ServerBuilder = builder;
            _ServerSupportEngine.UseWebSockets();
            return this;
        }
        public _WebSocket EngineInstall()
        {
            _ServerBuilder = WebApplication.CreateBuilder();
            _ServerSupportEngine = _ServerBuilder.Build();
            _ServerSupportEngine.UseWebSockets();
            return this;
        }
        public async Task StartServerEngine()
            => await _ServerSupportEngine.RunAsync(_ServerUrl ?? throw new Exception("E-Server:unknow url"));
        public async ValueTask DisposeAsync()
        {
            if(_ServerSupportEngine!=null)await _ServerSupportEngine.DisposeAsync();
        }
    }
}
