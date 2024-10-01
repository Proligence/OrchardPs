using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Newtonsoft.Json;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Console.UI;
using Proligence.SignalR.Core;

namespace Proligence.PowerShell.Web.Services {
    [Connection("commandstream")]
    public class CommandStreamConnection : PersistentConnection {
        private readonly IPsSessionManager _sessionManager;
        private readonly IConnectionManager _connectionManager;

        public CommandStreamConnection(IPsSessionManager sessionManager, IConnectionManager connectionManager) {
            _sessionManager = sessionManager;
            _connectionManager = connectionManager;
        }

        protected override Task OnConnected(IRequest request, string connectionId) {
            CreateSession(connectionId);
            return base.OnConnected(request, connectionId);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled) {
            var outputData = new OutputData {
                Output = "\r\nSession terminated!  Press ENTER to start a new session.\r\n"
            };

            Connection.Send(connectionId, outputData).Wait();
            EndSession(connectionId);
            return base.OnDisconnected(request, connectionId, stopCalled);
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data) {
            var session = CreateSession(connectionId);
            var input = JsonConvert.DeserializeObject<InputData>(data);

            if (input.Completion) {
                Connection.Send(
                    connectionId,
                    new OutputData {
                        Type = OutputType.Completion,
                        Data = session.GetCommandCompletion(input.Command, input.Position)
                    });
            }
            else {
                session.WriteInputBuffer(input.Command);
            }

            return base.OnReceived(request, connectionId, data);
        }

        private IPsSession CreateSession(string connectionId) {
            var session = _sessionManager.GetSession(connectionId);
            if (session == null) {
                var connection = new SignalRConsoleConnection(_connectionManager);
                session = _sessionManager.NewSession(connectionId, connection);
            }

            return session;
        }

        private void EndSession(string connectionId) {
            _sessionManager.CloseSession(connectionId);
        }
    }
}