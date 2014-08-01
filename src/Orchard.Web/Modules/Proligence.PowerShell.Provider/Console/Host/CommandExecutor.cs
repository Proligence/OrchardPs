namespace Proligence.PowerShell.Provider.Console.Host
{
    using System;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;

    public class CommandExecutor : ICommandExecutor
    {
        private readonly IPsSession session;
        private StringBuilder commandBuffer;
        private Pipeline pipeline;

        public CommandExecutor(IPsSession session)
        {
            this.session = session;
            this.commandBuffer = new StringBuilder();
        }

        public void Start()
        {
            this.session.DataReceived += this.OnSessionDataReceived;
        }

        public void Exit()
        {
            this.session.DataReceived -= this.OnSessionDataReceived;
        }

        private void ExecuteCommand()
        {
            string commandText = this.commandBuffer.ToString();
            this.commandBuffer = new StringBuilder();

            bool outputSent = false;

            try 
            {
                using (this.pipeline = this.session.Runspace.CreatePipeline(commandText)) 
                {
                    Collection<PSObject> result = this.pipeline.Invoke();
                    foreach (PSObject obj in result) 
                    {
                        this.session.ConsoleHost.UI.WriteLine(obj.ToString());
                        outputSent = true;
                    }

                    if (!outputSent) 
                    {
                        this.session.ConsoleHost.UI.WriteLine(string.Empty);
                    }
                }
            }
            catch (Exception ex) 
            {
                this.session.ConsoleHost.UI.WriteErrorLine(ex.Message);
            }
        }

        private void OnSessionDataReceived(object sender, DataReceivedEventArgs e)
        {
            string str = this.session.ReadInputBuffer();
            if (str != null)
            {
                this.commandBuffer.Append(str);

                if (!str.TrimEnd().EndsWith("`", StringComparison.Ordinal))
                {
                    this.ExecuteCommand();
                } 
            }
        }
    }
}