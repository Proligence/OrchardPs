// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardCmdletExtensions.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System;
    using System.Management.Automation;

    /// <summary>
    /// Implements extension methods for the <see cref="OrchardCmdlet"/> class.
    /// </summary>
    public static class OrchardCmdletExtensions
    {
        /// <summary>
        /// Writes a non-terminating error message to the PS provider.
        /// </summary>
        /// <param name="cmdlet">The cmdlet instance.</param>
        /// <param name="exception">The exception which contains the details of the error.</param>
        /// <param name="errorId">The error identifier (for PowerShell).</param>
        /// <param name="category">The error category (for PowerShell).</param>
        /// <param name="target">The target object of the current operation (for PowerShell), optional.</param>
        public static void WriteError(
            this IOrchardCmdlet cmdlet, 
            Exception exception, 
            string errorId, 
            ErrorCategory category, 
            object target = null)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            cmdlet.WriteError(errorRecord);
        }

        /// <summary>
        /// Writes a terminating error message to the PS provider.
        /// </summary>
        /// <param name="cmdlet">The cmdlet instance.</param>
        /// <param name="exception">The exception which contains the details of the error.</param>
        /// <param name="errorId">The error identifier (for PowerShell).</param>
        /// <param name="category">The error category (for PowerShell).</param>
        /// <param name="target">The target object of the current operation (for PowerShell), optional.</param>
        public static void ThrowTerminatingError(
            this IOrchardCmdlet cmdlet,
            Exception exception,
            string errorId,
            ErrorCategory category,
            object target = null)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            cmdlet.ThrowTerminatingError(errorRecord);
        }
    }
}