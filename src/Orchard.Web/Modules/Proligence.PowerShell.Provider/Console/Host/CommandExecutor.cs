namespace Proligence.PowerShell.Provider.Console.Host
{
    using System;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Text;

    public class CommandExecutor : ICommandExecutor
    {
        private readonly IPsSession session;
        private readonly StringBuilder commandBuffer;
        private Pipeline pipeline;
        private bool outputSent;

        public CommandExecutor(IPsSession session)
        {
            this.session = session;
            this.commandBuffer = new StringBuilder();
            this.outputSent = false;
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
            if (str != null)
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
                this.outputSent = true;
            }
        }

        private void OnPipelineStateChanged(object sender, PipelineStateEventArgs e)
        {
            PipelineState state = e.PipelineStateInfo.State;

            // Handled finished pipeline.
            if ((state == PipelineState.Completed) || (state == PipelineState.Failed) || (state == PipelineState.Stopped))
            {
                try
                {
                    // When pipeline finished, update current session's path
                    this.session.Path = this.session.Runspace.SessionStateProxy.Path.CurrentLocation.ToString();

                    // Display an error for failed pipelines.
                    if (state != PipelineState.Completed)
                    {
                        this.session.ConsoleHost.UI.WriteErrorLine(
                            "Command did not complete successfully. Reason: " + e.PipelineStateInfo.Reason);
                    }

                    // Dispose the current pipeline.
                    if (this.pipeline != null)
                    {
                        this.pipeline.Dispose();
                        this.pipeline = null;
                    }

                    // If the command did not generate any output, then write an empty line, to notify the console UI
                    // that the command finished.
                    if (!this.outputSent)
                    {
                        this.session.ConsoleHost.UI.WriteLine(string.Empty);
                    }
                }
                finally
                {
                    this.session.RunspaceLock.Set();
                    this.outputSent = false;
                }
            }
        }
    }
}