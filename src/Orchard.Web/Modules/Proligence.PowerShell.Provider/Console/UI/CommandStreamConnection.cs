namespace Proligence.PowerShell.Provider.Console.UI
{
    using System.Threading.Tasks;
    using Microsoft.AspNet.SignalR;
    using Newtonsoft.Json;
    using Proligence.SignalR.Core;

    [Connection("commandstream")]
    public class CommandStreamConnection : PersistentConnection 
    {
        private readonly IPsSessionManager sessionManager;

        public CommandStreamConnection(IPsSessionManager sessionManager) 
        {
            this.sessionManager = sessionManager;
        }

        protected override Task OnConnected(IRequest request, string connectionId) 
        {
            this.CreateSession(connectionId);
            return base.OnConnected(request, connectionId);
        }

        protected override bool AuthorizeRequest(IRequest request) 
        {
            return base.AuthorizeRequest(request);
        }

        protected override Task OnDisconnected(IRequest request, string connectionId, bool stopCalled) 
        {
            Connection.Send(connectionId, new OutputData { Output = "\r\nSession terminated!  Press ENTER to start a new session.\r\n" }).Wait();
            this.EndSession(connectionId);
            return base.OnDisconnected(request, connectionId, stopCalled);
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data) 
        {
            var session = this.CreateSession(connectionId);
            var input = JsonConvert.DeserializeObject<InputData>(data);

            if (input.Completion) 
            {
                Connection.Send(
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
            return this.sessionManager.GetSession(connectionId)
                ?? this.sessionManager.NewSession(connectionId);
        }

        private void EndSession(string connectionId)
        {
            this.sessionManager.CloseSession(connectionId);
        }
    }
}


