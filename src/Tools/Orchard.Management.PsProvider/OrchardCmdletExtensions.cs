using System;
using System.Management.Automation;

namespace Orchard.Management.PsProvider {
    public static class OrchardCmdletExtensions {
        public static void WriteError(
            this OrchardCmdlet cmdlet, Exception exception, string errorId, ErrorCategory category, object target = null) {

            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            cmdlet.WriteError(errorRecord);
        }

        public static void ThrowTerminatingError(
            this OrchardCmdlet cmdlet, Exception exception, string errorId, ErrorCategory category, object target = null) {

            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            cmdlet.ThrowTerminatingError(errorRecord);
        }
    }
}