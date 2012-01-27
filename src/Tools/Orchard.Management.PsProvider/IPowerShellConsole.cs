using System;
using System.Management.Automation;

namespace Orchard.Management.PsProvider {
    public interface IPowerShellConsole {
        void WriteError(Exception exception, string errorId, ErrorCategory category, object targetObject = null);
        void WriteWarning(string text);
        void WriteDebug(string text);
        void WriteVerbose(string text);
        void WriteLine();
    }
}