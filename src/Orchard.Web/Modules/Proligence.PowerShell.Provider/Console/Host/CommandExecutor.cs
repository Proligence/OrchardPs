using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using Proligence.PowerShell.Provider.Console.UI;

namespace Proligence.PowerShell.Provider.Console.Host {
    public class CommandExecutor : ICommandExecutor {
        private readonly IPsSession _session;
        private readonly StringBuilder _commandBuffer;
        private Pipeline _pipeline;

        public CommandExecutor(IPsSession session) {
            _session = session;
            _commandBuffer = new StringBuilder();
        }

        public void Start() {
            _session.DataReceived += OnSessionDataReceived;
        }

        public void Exit() {
            _session.DataReceived -= OnSessionDataReceived;
        }

        private void ExecuteCommandFromBuffer() {
            string commandText = _commandBuffer.ToString();
            _commandBuffer.Clear();

            try {
                _session.RunspaceLock.WaitOne();
                _pipeline = _session.Runspace.CreatePipeline(commandText);
                _pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
                _pipeline.Commands.Add(new Command("Out-Default", false, true));
                _pipeline.Output.DataReady += OnPipelineOutputDataReady;
                _pipeline.StateChanged += OnPipelineStateChanged;
                _pipeline.Input.Close();
                _pipeline.InvokeAsync();
            }
            catch (Exception ex) {
                _session.ConsoleHost.UI.WriteErrorLine(ex.Message);
                _session.RunspaceLock.Set();
            }
        }

        private void OnSessionDataReceived(object sender, DataReceivedEventArgs e) {
            string str = _session.ReadInputBuffer();
            if (str == null) {
                return;
            }

            bool aborted = str.Contains("\u0003");
            if (!aborted && (_pipeline != null) && (_pipeline.PipelineStateInfo.State != PipelineState.NotStarted)) {
                return;
            }

            if (aborted) {
                if (_pipeline != null
                    && _pipeline.PipelineStateInfo.State != PipelineState.Stopped
                    && _pipeline.PipelineStateInfo.State != PipelineState.Stopping) {
                    _pipeline.StopAsync();
                }
                else {
                    // Sending information about finishing command
                    _session.Sender(new OutputData {Finished = true});
                    _session.SignalInputProcessed();
                }
            }
            else {
                _commandBuffer.Append(str);

                if (!str.TrimEnd().EndsWith("`", StringComparison.Ordinal)) {
                    ExecuteCommandFromBuffer();
                }
            }
        }

        private void OnPipelineOutputDataReady(object sender, EventArgs eventArgs) {
            foreach (PSObject result in _pipeline.Output.NonBlockingRead()) {
                var resultString = (string) LanguagePrimitives.ConvertTo(result, typeof (string));
                _session.ConsoleHost.UI.WriteLine(resultString);
            }

            _session.SignalInputProcessed();
        }

        private void OnPipelineStateChanged(object sender, PipelineStateEventArgs e) {
            if (e.PipelineStateInfo.State != PipelineState.Running) {
                _session.Path = _session.Runspace.SessionStateProxy.Path.CurrentLocation.ToString();
            }

            // Handled finished pipeline.
            PipelineState state = e.PipelineStateInfo.State;
            if ((state == PipelineState.Completed) || (state == PipelineState.Failed) ||
                (state == PipelineState.Stopped)) {
                try {
                    // Display an error for failed pipelines.
                    if (state == PipelineState.Failed) {
                        _session.ConsoleHost.UI.WriteErrorLine(
                            "Command did not complete successfully. Reason: " + e.PipelineStateInfo.Reason);
                    }

                    if (state == PipelineState.Stopped) {
                        _session.ConsoleHost.UI.WriteWarningLine(
                            "Command '" + _pipeline.Commands[0].CommandText + "' has been cancelled.");
                    }

                    // Dispose the current pipeline.
                    if (_pipeline != null) {
                        _pipeline.Dispose();
                        _pipeline = null;
                    }
                }
                finally {
                    try {
                        // Sending information about finishing command
                        _session.Sender(new OutputData {Finished = true});
                    }
                    finally {
                        _session.RunspaceLock.Set();
                    }
                }
            }
        }
    }
}