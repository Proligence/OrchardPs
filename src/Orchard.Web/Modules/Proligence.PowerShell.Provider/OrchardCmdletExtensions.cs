namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Management.Automation;
    using Orchard.Validation;

    public static class OrchardCmdletExtensions
    {
        /// <summary>
        /// Writes a non-terminating error message to the PS provider.
        /// </summary>
        public static void WriteError(
            this IOrchardCmdlet cmdlet, 
            Exception exception, 
            string errorId, 
            ErrorCategory category, 
            object target = null)
        {
            Argument.ThrowIfNull(cmdlet, "cmdlet");
            
            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            cmdlet.WriteError(errorRecord);
        }

        /// <summary>
        /// Writes a terminating error message to the PS provider.
        /// </summary>
        public static void ThrowTerminatingError(
            this IOrchardCmdlet cmdlet,
            Exception exception,
            string errorId,
            ErrorCategory category,
            object target = null)
        {
            Argument.ThrowIfNull(cmdlet, "cmdlet");
            
            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            cmdlet.ThrowTerminatingError(errorRecord);
        }
    }
}