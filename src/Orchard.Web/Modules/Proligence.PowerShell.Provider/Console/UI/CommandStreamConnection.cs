using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Orchard.Environment;
using Proligence.SignalR.Core;

namespace Proligence.PowerShell.Provider.Console.UI
{
    [Connection("commandstream")]
    public class CommandStreamConnection : PersistentConnection {
        private readonly IHostEnvironment _environment;
        private readonly IPsSessionManager _sessionManager;

        public CommandStreamConnection(IHostEnvironment environment, IPsSessionManager sessionManager) {
            _environment = environment;
            _sessionManager = sessionManager;
        }

        protected override Task OnConnected(IRequest request, string connectionId) {
            CreateSession(connectionId);
            return base.OnConnected(request, connectionId);
        }

        private IPsSession CreateSession(string connectionId) {
            return _sessionManager.GetSession(connectionId) 
                ?? _sessionManager.NewSession(connectionId);
        }

        private void EndSession(string connectionId) {
            _sessionManager.CloseSession(connectionId);
        }

        protected override bool AuthorizeRequest(IRequest request) {
            return base.AuthorizeRequest(request);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId) {

            Connection.Send(connectionId, new { Output = "\r\nSession terminated!  Press ENTER to start a new session.\r\n" }).Wait();
            EndSession(connectionId);
            return base.OnDisconnected(request, connectionId);
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data) {
            var session = CreateSession(connectionId);
         
            session.WriteInputBuffer(data);

            return base.OnReceived(request, connectionId, data);
        }
    }
}


