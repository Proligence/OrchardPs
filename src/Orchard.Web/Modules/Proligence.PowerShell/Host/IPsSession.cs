namespace Proligence.PowerShell.Host
{
    using System;
    using Orchard.Management.PsProvider.Console;

    /// <summary>
    /// Represents a PowerShell user session.
    /// </summary>
    public interface IPsSession : IDisposable
    {
        /// <summary>
        /// Gets the console host which provides input/output to the PowerShell engine.
        /// </summary>
        ConsoleHost ConsoleHost { get; }
    }
}