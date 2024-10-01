using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using System.Threading;
using Proligence.PowerShell.Provider.Console.Host;

namespace Proligence.PowerShell.Provider.Console.UI {
    public class ConsoleHostUserInterface : PSHostUserInterface {
        private readonly ConsoleHost _consoleHost;
        private readonly AutoResetEvent _promptLock;
        private readonly ConsoleHostRawUserInterface _rawUserInterface;

        public ConsoleHostUserInterface(ConsoleHost consoleHost) {
            _consoleHost = consoleHost;
            _promptLock = new AutoResetEvent(false);
            _rawUserInterface = new ConsoleHostRawUserInterface(this);
        }

        public EventWaitHandle PromptLock {
            get { return _promptLock; }
        }

        public override PSHostRawUserInterface RawUI {
            get { return _rawUserInterface; }
        }

        public override string ReadLine() {
            return _consoleHost.Session.ReadInputBuffer();
        }

        public override SecureString ReadLineAsSecureString() {
            var line = _consoleHost.Session.ReadInputBuffer();
            var secure = new SecureString();

            foreach (var c in line) {
                secure.AppendChar(c);
            }

            return secure;
        }

        public override void Write(string value) {
            _consoleHost.Session.Sender(
                new OutputData {
                    Output = value
                });
        }

        public override void Write(ConsoleColor foregroundColor, ConsoleColor backgroundColor, string value) {
            _consoleHost.Session.Sender(
                new OutputData {
                    Output = value,
                    BackColor = backgroundColor.ToString(),
                    ForeColor = foregroundColor.ToString()
                });
        }

        public override void WriteLine(string value) {
            _consoleHost.Session.Sender(
                new OutputData {
                    Output = value,
                    NewLine = true
                });
        }

        public override void WriteErrorLine(string value) {
            _consoleHost.Session.Sender(
                new OutputData {
                    Output = value,
                    Type = OutputType.Error,
                    NewLine = true
                });
        }

        public override void WriteDebugLine(string message) {
            _consoleHost.Session.Sender(
                new OutputData {
                    Output = message,
                    Type = OutputType.Debug,
                    NewLine = true
                });
        }

        public override void WriteProgress(long sourceId, ProgressRecord record) {
            _consoleHost.Session.Sender(
                new OutputData {
                    Type = OutputType.Progress,
                    Data = record
                });
        }

        public override void WriteVerboseLine(string message) {
            _consoleHost.Session.Sender(
                new OutputData {
                    Output = message,
                    Type = OutputType.Verbose,
                    NewLine = true
                });
        }

        public override void WriteWarningLine(string message) {
            _consoleHost.Session.Sender(
                new OutputData {
                    Output = message,
                    Type = OutputType.Warning,
                    NewLine = true
                });
        }

        public override Dictionary<string, PSObject> Prompt(string caption, string message, Collection<FieldDescription> descriptions) {
            var dict = new Dictionary<string, PSObject>();

            _promptLock.Reset();
            _consoleHost.Session.DataReceived += SessionOnDataReceived;

            WriteLine(caption);
            WriteLine(message);
            WriteLine("---");

            foreach (var desc in descriptions) {
                _consoleHost.Session.Sender(
                    new OutputData {
                        Prompt = desc.Name + ": ",
                        NewLine = true
                    });

                if (_promptLock.WaitOne()) {
                    try {
                        dict.Add(desc.Name, PSObject.AsPSObject(ReadLine()));
                    }
                    finally {
                        _promptLock.Reset();
                    }
                }
            }

            WriteLine("---");
            _consoleHost.Session.DataReceived -= SessionOnDataReceived;
            return dict;
        }

        public override PSCredential PromptForCredential(string caption, string message, string userName, string targetName) {
            throw new NotImplementedException();
        }

        public override PSCredential PromptForCredential(
            string caption,
            string message,
            string userName,
            string targetName,
            PSCredentialTypes allowedCredentialTypes,
            PSCredentialUIOptions options) {
            throw new NotImplementedException();
        }

        public override int PromptForChoice(string caption, string message, Collection<ChoiceDescription> choices, int defaultChoice) {
            WriteLine(caption);
            WriteLine(message);

            WriteLine("---");

            int i = 0;
            int retVal = defaultChoice;

            foreach (var choice in choices) {
                Write(i + ": " + choice.Label);
                if (i == defaultChoice) {
                    Write(" (default)");
                }

                WriteLine();
                i++;
            }

            WriteLine("---");
            WriteLine("Which one do you choose (number)?: ");

            _promptLock.Reset();
            _consoleHost.Session.DataReceived += SessionOnDataReceived;
            _consoleHost.Session.Sender(
                new OutputData {
                    Prompt = "I choose > ",
                    NewLine = true
                });

            if (_promptLock.WaitOne()) {
                try {
                    retVal = Convert.ToInt32(ReadLine());
                }
                finally {
                    _promptLock.Reset();
                }
            }

            WriteLine("---");
            _consoleHost.Session.DataReceived -= SessionOnDataReceived;
            return retVal;
        }

        private void SessionOnDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs) {
            _promptLock.Set();
        }
    }
}