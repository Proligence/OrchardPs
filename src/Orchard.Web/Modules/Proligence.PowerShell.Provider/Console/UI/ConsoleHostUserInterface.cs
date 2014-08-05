namespace Proligence.PowerShell.Provider.Console.UI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using System.Security;
    using System.Threading;
    using Proligence.PowerShell.Provider.Console.Host;

    public class ConsoleHostUserInterface : PSHostUserInterface
    {
        private readonly ConsoleHost consoleHost;
        private readonly AutoResetEvent promptLock;

        public ConsoleHostUserInterface(ConsoleHost consoleHost)
        {
            this.consoleHost = consoleHost;
            this.promptLock = new AutoResetEvent(false);
        }

        public EventWaitHandle PromptLock
        {
            get { return this.promptLock; }
        }

        public override PSHostRawUserInterface RawUI
        {
            get { return null; }
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

            this.promptLock.Reset();
            this.consoleHost.Session.DataReceived += this.SessionOnDataReceived;

            this.WriteLine(caption);
            this.WriteLine(message);
            this.WriteLine("---");

            foreach (var desc in descriptions) 
            {
                this.consoleHost.Session.Sender(
                    new OutputData 
                    {
                        Path = desc.Name + ": ",
                        NewLine = true
                    });

                if (this.promptLock.WaitOne()) 
                {
                    try 
                    {
                        dict.Add(desc.Name, PSObject.AsPSObject(this.ReadLine()));
                    }
                    finally 
                    {
                        this.promptLock.Reset();
                    }
                }
            }

            this.WriteLine("---");
            this.consoleHost.Session.DataReceived -= this.SessionOnDataReceived;
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
            this.WriteLine(caption);
            this.WriteLine(message);

            this.WriteLine("---");

            int i = 0;
            int retVal = defaultChoice;

            foreach (var choice in choices) 
            {
                this.Write(i + ": " + choice.Label);
                if (i == defaultChoice) 
                {
                    this.Write(" (default)");
                }

                this.WriteLine();
                i++;
            }

            this.WriteLine("---");
            this.WriteLine("Which one do you choose (number)?: ");

            this.promptLock.Reset();
            this.consoleHost.Session.DataReceived += this.SessionOnDataReceived;
                this.consoleHost.Session.Sender(
                new OutputData
                {
                    Path = "I choose > ",
                    NewLine = true
                });

            if (this.promptLock.WaitOne()) 
            {
                try 
                {
                    retVal = Convert.ToInt32(this.ReadLine());
                }
                finally
                {
                    this.promptLock.Reset();
                }
            }

            this.WriteLine("---");
            this.consoleHost.Session.DataReceived -= this.SessionOnDataReceived;
            return retVal;
        }

        private void SessionOnDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            this.promptLock.Set();
        }
    }
}