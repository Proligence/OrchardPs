using System;
using System.Text;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Console.UI;

namespace Orchard.Tests.PowerShell.Infrastructure {
    public class TestConsoleConnection : MarshalByRefObject, IConsoleConnection {
        public TestConsoleConnection() {
            VerboseOutput = new StringBuilder();
            WarningOutput = new StringBuilder();
            DebugOutput = new StringBuilder();
            ErrorOutput = new StringBuilder();
            Output = new StringBuilder();
        }

        public StringBuilder VerboseOutput { get; private set; }
        public StringBuilder WarningOutput { get; private set; }
        public StringBuilder DebugOutput { get; private set; }
        public StringBuilder ErrorOutput { get; private set; }
        public StringBuilder Output { get; private set; }
        public OutputData LastOutputData { get; private set; }

        public void Initialize() {
        }

        public override object InitializeLifetimeService() {
            return null;
        }

        public void Send(string connectionId, OutputData data) {
            if (data != null) {
                LastOutputData = data;

                if (data.Output != null) {
                    string output = data.NewLine
                        ? data.Output + Environment.NewLine
                        : data.Output;

                    if (data.Type == OutputType.Verbose) {
                        VerboseOutput.Append(output);
                    }
                    else if (data.Type == OutputType.Warning) {
                        WarningOutput.Append(output);
                    }
                    else if (data.Type == OutputType.Debug) {
                        DebugOutput.Append(output);
                    }
                    else if (data.Type == OutputType.Error) {
                        ErrorOutput.Append(output);
                    }
                    else {
                        Output.Append(output);
                    }
                }
            }
        }

        public void Reset() {
            VerboseOutput.Clear();
            WarningOutput.Clear();
            DebugOutput.Clear();
            ErrorOutput.Clear();
            Output.Clear();
        }
    }
}