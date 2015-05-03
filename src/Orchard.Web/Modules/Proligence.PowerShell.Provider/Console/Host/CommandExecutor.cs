namespace Proligence.PowerShell.Provider.Console.Host
{
    using System;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;
    using Proligence.PowerShell.Provider.Console.UI;

    public class CommandExecutor : ICommandExecutor
    {
        private readonly IPsSession session;
        private readonly StringBuilder commandBuffer;
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

        private void ExecuteCommandFromBuffer()
        {
            string commandText = this.commandBuffer.ToString();
            this.commandBuffer.Clear();

            try
            {
                this.session.RunspaceLock.WaitOne();
                this.pipeline = this.session.Runspace.CreatePipeline(commandText);
                this.pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                this.pipeline.Commands.Add(new Command("Out-Default", false, true));
                this.pipeline.Output.DataReady += this.OnPipelineOutputDataReady;
                this.pipeline.StateChanged += this.OnPipelineStateChanged;
                this.pipeline.Input.Close();
                this.pipeline.InvokeAsync();
            }
            catch (Exception ex)
            {
                this.session.ConsoleHost.UI.WriteErrorLine(ex.Message);
                this.session.RunspaceLock.Set();
            }
        }

        private void OnSessionDataReceived(object sender, DataReceivedEventArgs e)
        {
            string str = this.session.ReadInputBuffer();
            if (str == null) 
            {
                return;
            }

            bool aborted = str.Contains("\u0003");
            if (!aborted && (this.pipeline != null) && (this.pipeline.PipelineStateInfo.State != PipelineState.NotStarted))
            {
                return;
            }

            if (aborted) 
            {
                if (this.pipeline != null 
                    && this.pipeline.PipelineStateInfo.State != PipelineState.Stopped
                    && this.pipeline.PipelineStateInfo.State != PipelineState.Stopping)
                {
                    this.pipeline.StopAsync();
                }
                else
                {
                    // Sending information about finishing command
                    this.session.Sender(new OutputData { Finished = true });
                    this.session.SignalInputProcessed();
                }
            }
            else 
            {
                this.commandBuffer.Append(str);

                if (!str.TrimEnd().EndsWith("`", StringComparison.Ordinal))
                {
                    this.ExecuteCommandFromBuffer();
                }
            }
        }

        private void OnPipelineOutputDataReady(object sender, EventArgs eventArgs)
        {
            foreach (PSObject result in this.pipeline.Output.NonBlockingRead())
            {
                var resultString = (string)LanguagePrimitives.ConvertTo(result, typeof(string));
                this.session.ConsoleHost.UI.WriteLine(resultString);
            }

            this.session.SignalInputProcessed();
        }

        private void OnPipelineStateChanged(object sender, PipelineStateEventArgs e)
        {
            if (e.PipelineStateInfo.State != PipelineState.Running)
            {
                this.session.Path = this.session.Runspace.SessionStateProxy.Path.CurrentLocation.ToString();
            }

            // Handled finished pipeline.
            PipelineState state = e.PipelineStateInfo.State;
            if ((state == PipelineState.Completed) || (state == PipelineState.Failed) || (state == PipelineState.Stopped))
            {
                try
                {
                    // Display an error for failed pipelines.
                    if (state == PipelineState.Failed)
                    {
                        this.session.ConsoleHost.UI.WriteErrorLine(
                            "Command did not complete successfully. Reason: " + e.PipelineStateInfo.Reason);
                    }

                    if (state == PipelineState.Stopped)
                    {
                        this.session.ConsoleHost.UI.WriteWarningLine(
                            "Command '" + this.pipeline.Commands[0].CommandText + "' has been cancelled.");
                    }

                    // Dispose the current pipeline.
                    if (this.pipeline != null)
                    {
                        this.pipeline.Dispose();
                        this.pipeline = null;
                    }
                }
                finally
                {
                    try
                    {
                        // Sending information about finishing command
                        this.session.Sender(new OutputData { Finished = true });
                    }
                    finally
                    {
                        this.session.RunspaceLock.Set();
                    }
                }
            }
        }
    }
}