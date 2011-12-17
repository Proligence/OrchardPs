using System;
using System.Diagnostics;
using System.Management.Automation;

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
            Trace.WriteLine(exception.ToString());
        }

        public void WriteWarning(string text) {
            _provider.WriteDebug(text);
            Trace.WriteLine("WARNING: " + text);
        }

        public void WriteDebug(string text) {
            _provider.WriteDebug(text);
            Trace.WriteLine("DEBUG  : " + text);
        }

        public void WriteVerbose(string text) {
            _provider.WriteDebug(text);
            Trace.WriteLine("VERBOSE: " + text);
        }
    }
}