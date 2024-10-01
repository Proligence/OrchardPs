using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Console.UI;

namespace Proligence.PowerShell.Web.Services {
    public class SignalRConsoleConnection : IConsoleConnection {
        private readonly IConnectionManager _connectionManager;
        private IPersistentConnectionContext _ctx;

        public SignalRConsoleConnection(IConnectionManager connectionManager) {
            _connectionManager = connectionManager;
        }

        public void Initialize() {
            _ctx = _connectionManager.GetConnectionContext<CommandStreamConnection>();
        }

        public void Send(string connectionId, OutputData data) {
            _ctx.Connection.Send(connectionId, data).Wait();
        }
    }
}