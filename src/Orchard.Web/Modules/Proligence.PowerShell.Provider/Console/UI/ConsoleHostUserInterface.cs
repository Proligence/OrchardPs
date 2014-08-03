using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using Proligence.PowerShell.Provider.Console.Host;

namespace Proligence.PowerShell.Provider.Console.UI
{
    public class ConsoleHostUserInterface : PSHostUserInterface
    {
        private readonly ConsoleHost consoleHost;
        private readonly ConsoleHostRawUserInterface rawUi;

        public ConsoleHostUserInterface(ConsoleHost consoleHost)
        {
            this.consoleHost = consoleHost;
        }

        public override PSHostRawUserInterface RawUI
        {
            get { return this.rawUi; }
        }

        public override string ReadLine() {
            // TODO: Implement waiting for OnDataReceived event
            return consoleHost.Session.ReadInputBuffer();
        }

        public override SecureString ReadLineAsSecureString() {
            // TODO: Implement waiting for OnDataReceived event
            var line = consoleHost.Session.ReadInputBuffer();
            var secure = new SecureString();

            foreach (var c in line) {
                secure.AppendChar(c);
            }

            return secure;
        }

        public override void Write(string value)
        {
            consoleHost.Session.Sender(
                new OutputData {
                    Output = value
                });
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value) {
            consoleHost.Session.Sender(
                new OutputData {
                    Output = value,
                    BackColor = backgroundColor.ToString(),
                    ForeColor = foregroundColor.ToString()
                });
        }

        public override void WriteLine(string value)
        {
            consoleHost.Session.Sender(
                new OutputData {
                    Output = value + Environment.NewLine
                });
        }

        public override void WriteErrorLine(string value)
        {
            consoleHost.Session.Sender(
                new OutputData
                {
                    Output = value,
                    Type = OutputType.Error
                });
        }

        public override void WriteDebugLine(string message) 
        {
            consoleHost.Session.Sender(
                new OutputData {
                    Output = message,
                    Type = OutputType.Debug
                });
        }

        public override void WriteProgress(long sourceId, ProgressRecord record) 
        {
            consoleHost.Session.Sender(
                new OutputData {
                Type = OutputType.Progress,
                Data = record
            });
        }

        public override void WriteVerboseLine(string message)
        {
            consoleHost.Session.Sender(
                new OutputData
                {
                    Output = message,
                    Type = OutputType.Verbose
                });
        }

        public override void WriteWarningLine(string message)
        {
            consoleHost.Session.Sender(
                new OutputData
                {
                    Output = message,
                    Type = OutputType.Warning
                });
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions) 
        {
            var dict = new Dictionary<string, PSObject>();

            WriteLine(caption);
            WriteLine(message);
            WriteLine("-------------------------------");

            foreach (var desc in descriptions) {
                WriteLine(desc.Label + ": ");
                dict.Add(desc.Name, PSObject.AsPSObject(ReadLine()));
            }

            WriteLine("-------------------------------");
            return dict;
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName)
        {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(
            string caption,
            string message,
            string userName,
            string targetName,
            PSCredentialTypes allowedCredentialTypes,
            PSCredentialUIOptions options)
        {
            throw new NotImplementedException();
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice)
        {
            return defaultChoice;
        }
    }
}