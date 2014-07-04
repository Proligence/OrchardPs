// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerShellConsole.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Core
{
    using System;
    using System.Diagnostics;
    using System.Management.Automation;
    using System.Management.Automation.Host;
    using Proligence.PowerShell.Vfs.Provider;

    /// <summary>
    /// Provides access to the PowerShell-controled console output.
    /// </summary>
    public class PowerShellConsole : IPowerShellConsole 
    {
        /// <summary>
        /// The PowerShell VFS PS provider instance.
        /// </summary>
        private readonly VfsProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellConsole"/> class.
        /// </summary>
        /// <param name="provider">The VFS-based PS provider instance.</param>
        public PowerShellConsole(VfsProvider provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Writes an error-level message to the console.
        /// </summary>
        /// <param name="exception">The exception which contains the error details.</param>
        /// <param name="errorId">The error identifier (for PowerShell).</param>
        /// <param name="category">The error category (for PowerShell).</param>
        /// <param name="targetObject">The operation's target object (for PowerShell), optional.</param>
        public void WriteError(Exception exception, string errorId, ErrorCategory category, object targetObject = null)
        {
            if (exception == null)
            {
                throw new ArgumentNullException("exception");
            }

            var errorRecord = new ErrorRecord(exception, errorId, category, targetObject);
            this.provider.WriteError(errorRecord);

            PSHostUserInterface ui = this.GetHostUi();
            if (ui != null) 
            {
                ui.WriteErrorLine(exception.Message);
            }

            Trace.WriteLine("ERROR: " + exception);
        }

        /// <summary>
        /// Writes a warning-level message to the console.
        /// </summary>
        /// <param name="text">The text which will be written to the console.</param>
        public void WriteWarning(string text) 
        {
            PSHostUserInterface ui = this.GetHostUi();
            if (ui != null) 
            {
                ui.WriteWarningLine(text);
            }
            
            Trace.WriteLine("WARNING: " + text);
        }

        /// <summary>
        /// Writes a debug-level message to the console.
        /// </summary>
        /// <param name="text">The text which will be written to the console.</param>
        public void WriteDebug(string text) 
        {
            PSHostUserInterface ui = this.GetHostUi();
            if (ui != null) 
            {
                ui.WriteDebugLine(text);
            }

            Trace.WriteLine("DEBUG  : " + text);
        }

        /// <summary>
        /// Writes a verbose-level message to the console.
        /// </summary>
        /// <param name="text">The text which will be written to the console.</param>
        public void WriteVerbose(string text) 
        {
            PSHostUserInterface ui = this.GetHostUi();
            if (ui != null) 
            {
                ui.WriteVerboseLine(text);
            }

            Trace.WriteLine("VERBOSE: " + text);
        }

        /// <summary>
        /// Writes a new line to the console.
        /// </summary>
        public void WriteLine() 
        {
            PSHostUserInterface ui = this.GetHostUi();
            if (ui != null) 
            {
                ui.WriteLine();
            }
        }

        /// <summary>
        /// Gets the <see cref="PSHostUserInterface"/> object which provides access to the application's console.
        /// </summary>
        /// <returns>The <see cref="PSHostUserInterface"/> object.</returns>
        private PSHostUserInterface GetHostUi() 
        {
            if (this.provider.Host != null) 
            {
                return this.provider.Host.UI;
            }

            return null;
        }
    }
}