// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPowerShellConsole.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Core
{
    using System;
    using System.Management.Automation;

    /// <summary>
    /// Provides access to the PowerShell-controled console output.
    /// </summary>
    public interface IPowerShellConsole 
    {
        /// <summary>
        /// Writes an error-level message to the console.
        /// </summary>
        /// <param name="exception">The exception which contains the error details.</param>
        /// <param name="errorId">The error identifier (for PowerShell).</param>
        /// <param name="category">The error category (for PowerShell).</param>
        /// <param name="targetObject">The operation's target object (for PowerShell), optional.</param>
        void WriteError(Exception exception, string errorId, ErrorCategory category, object targetObject = null);

        /// <summary>
        /// Writes a warning-level message to the console.
        /// </summary>
        /// <param name="text">The text which will be written to the console.</param>
        void WriteWarning(string text);

        /// <summary>
        /// Writes a debug-level message to the console.
        /// </summary>
        /// <param name="text">The text which will be written to the console.</param>
        void WriteDebug(string text);

        /// <summary>
        /// Writes a verbose-level message to the console.
        /// </summary>
        /// <param name="text">The text which will be written to the console.</param>
        void WriteVerbose(string text);

        /// <summary>
        /// Writes a new line to the console.
        /// </summary>
        void WriteLine();
    }
}