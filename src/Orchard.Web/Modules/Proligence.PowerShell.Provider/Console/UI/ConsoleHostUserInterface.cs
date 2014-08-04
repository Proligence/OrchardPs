namespace Proligence.PowerShell.Provider.Console.UI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using System.Security;

    using Proligence.PowerShell.Provider.Console.Host;

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

        public override string ReadLine() 
        {
            return this.consoleHost.Session.ReadInputBuffer();
        }

        public override SecureString ReadLineAsSecureString() 
        {
            var line = this.consoleHost.Session.ReadInputBuffer();
            var secure = new SecureString();

            foreach (var c in line) 
            {
                secure.AppendChar(c);
            }

            return secure;
        }

        public override void Write(string value)
        {
            this.consoleHost.Session.Sender(
                new OutputData 
                {
                    Output = value
                });
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value) 
        {
            this.consoleHost.Session.Sender(
                new OutputData 
                {
                    Output = value,
                    BackColor = backgroundColor.ToString(),
                    ForeColor = foregroundColor.ToString()
                });
        }

        public override void WriteLine(string value)
        {
            this.consoleHost.Session.Sender(
                new OutputData 
                {
                    Output = value,
                    NewLine = true
                });
        }

        public override void WriteErrorLine(string value)
        {
            this.consoleHost.Session.Sender(
                new OutputData
                {
                    Output = value,
                    Type = OutputType.Error,
                    NewLine = true
                });
        }

        public override void WriteDebugLine(string message) 
        {
            this.consoleHost.Session.Sender(
                new OutputData 
                {
                    Output = message,
                    Type = OutputType.Debug,
                    NewLine = true
                });
        }

        public override void WriteProgress(long sourceId, ProgressRecord record) 
        {
            this.consoleHost.Session.Sender(
                new OutputData 
                {
                    Type = OutputType.Progress,
                    Data = record
                });
        }

        public override void WriteVerboseLine(string message)
        {
            this.consoleHost.Session.Sender(
                new OutputData
                {
                    Output = message,
                    Type = OutputType.Verbose,
                    NewLine = true
                });
        }

        public override void WriteWarningLine(string message)
        {
            this.consoleHost.Session.Sender(
                new OutputData
                {
                    Output = message,
                    Type = OutputType.Warning,
                    NewLine = true
                });
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions) 
        {
            var dict = new Dictionary<string, PSObject>();

            this.WriteLine(caption);
            this.WriteLine(message);
            this.WriteLine("-------------------------------");

            foreach (var desc in descriptions) 
            {
                this.WriteLine(desc.Label + ": ");
                dict.Add(desc.Name, PSObject.AsPSObject(this.ReadLine()));
            }

            this.WriteLine("-------------------------------");
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