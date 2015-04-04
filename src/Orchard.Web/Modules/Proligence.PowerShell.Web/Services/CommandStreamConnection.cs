namespace Proligence.PowerShell.Web.Services
{
    using System.Threading.Tasks;
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Infrastructure;
    using Newtonsoft.Json;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Console.UI;
    using Proligence.SignalR.Core;

    [Connection("commandstream")]
    public class CommandStreamConnection : PersistentConnection 
    {
        private readonly IPsSessionManager sessionManager;
        private readonly IConnectionManager connectionManager;

        public CommandStreamConnection(IPsSessionManager sessionManager, IConnectionManager connectionManager)
        {
            this.sessionManager = sessionManager;
            this.connectionManager = connectionManager;
        }

        protected override Task OnConnected(IRequest request, string connectionId) 
        {
            this.CreateSession(connectionId);
            return base.OnConnected(request, connectionId);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled) 
        {
            var outputData = new OutputData { Output = "\r\nSession terminated!  Press ENTER to start a new session.\r\n" };
            this.Connection.Send(connectionId, outputData).Wait();
            this.EndSession(connectionId);
            return base.OnDisconnected(request, connectionId, stopCalled);
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data) 
        {
            var session = this.CreateSession(connectionId);
            var input = JsonConvert.DeserializeObject<InputData>(data);

            if (input.Completion) 
            {
                this.Connection.Send(
                    connectionId, 
                    new OutputData 
                    { 
                        Type = OutputType.Completion, 
                        Data = session.GetCommandCompletion(input.Command, input.Position)
                    });
            }
            else 
            {
                session.WriteInputBuffer(input.Command);     
            }

            return base.OnReceived(request, connectionId, data);
        }

        private IPsSession CreateSession(string connectionId)
        {
            var session = this.sessionManager.GetSession(connectionId);
            if (session == null)
            {
                var connection = new SignalRConsoleConnection(this.connectionManager);
                session = this.sessionManager.NewSession(connectionId, connection);
            }

            return session;
        }

        private void EndSession(string connectionId)
        {
            this.sessionManager.CloseSession(connectionId);
        }
    }
}