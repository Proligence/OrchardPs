using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace Orchard.Management.PsProvider
{
    internal class PowerShellConsole : IPowerShellConsole {
        private readonly OrchardProvider _provider;

        public PowerShellConsole(OrchardProvider provider) {
            _provider = provider;
        }

        public void WriteError(Exception exception, string errorId, ErrorCategory category, object targetObject = null) {
            var errorRecord = new ErrorRecord(exception, errorId, category, targetObject);
            _provider.WriteError(errorRecord);

            PSHostUserInterface ui = GetHostUi();
            if (ui != null) {
                ui.WriteErrorLine(exception.Message);
            }

            Trace.WriteLine("ERROR: " + exception);
        }

        public void WriteWarning(string text) {
            PSHostUserInterface ui = GetHostUi();
            if (ui != null) {
                ui.WriteWarningLine(text);
            }
            
            Trace.WriteLine("WARNING: " + text);
        }

        public void WriteDebug(string text) {
            PSHostUserInterface ui = GetHostUi();
            if (ui != null) {
                ui.WriteDebugLine(text);
            }

            Trace.WriteLine("DEBUG  : " + text);
        }

        public void WriteVerbose(string text) {
            PSHostUserInterface ui = GetHostUi();
            if (ui != null) {
                ui.WriteVerboseLine(text);
            }

            Trace.WriteLine("VERBOSE: " + text);
        }

        public void WriteLine() {
            PSHostUserInterface ui = GetHostUi();
            if (ui != null) {
                ui.WriteLine();
            }
        }


        private PSHostUserInterface GetHostUi() {
            if (_provider.Host != null) {
                return _provider.Host.UI;
            }

            return null;
        }
    }
}