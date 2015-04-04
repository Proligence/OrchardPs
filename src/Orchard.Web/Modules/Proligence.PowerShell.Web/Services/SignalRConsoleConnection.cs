namespace Proligence.PowerShell.Web.Services
{
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Infrastructure;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Console.UI;

    public class SignalRConsoleConnection : IConsoleConnection
    {
        private readonly IConnectionManager connectionManager;
        private IPersistentConnectionContext ctx;

        public SignalRConsoleConnection(IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
        }

        public void Initialize()
        {
            this.ctx = this.connectionManager.GetConnectionContext<CommandStreamConnection>();
        }

        public void Send(string connectionId, OutputData data)
        {
            this.ctx.Connection.Send(connectionId, data).Wait();
        }
    }
}