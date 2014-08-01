namespace Proligence.PowerShell.Provider.Console
{
    using System;
    using System.Management.Automation.Runspaces;
    using System.Threading;
    using Proligence.PowerShell.Provider.Console.Host;
    using Proligence.PowerShell.Provider.Console.UI;

    /// <summary>
    /// Represents a PowerShell user session.
    /// </summary>
    public interface IPsSession : IDisposable
    {
        event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Gets the console host which provides input/output to the PowerShell engine.
        /// </summary>
        ConsoleHost ConsoleHost { get; }

        /// <summary>
        /// Gets the PowerShell runspace associated with the session.
        /// </summary>
        Runspace Runspace { get; }

        /// <summary>
        /// Gets the lock which must be aquired before acessing the session's runspace.
        /// </summary>
        EventWaitHandle RunspaceLock { get; }

        /// <summary>
        ///  Delegate used for sending messages up to the user console.
        /// </summary>
        Action<OutputData> Sender { get; }

        /// <summary>
        /// SignalR connection identifier for this particular session.
        /// </summary>
        string ConnectionId { get; }

        /// <summary>
        /// Gets the current session's runspace absolute path.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Reads line of string from input buffer.
        /// </summary>
        /// <returns></returns>
        string ReadInputBuffer();

        /// <summary>
        /// Writes a line to the input buffer.
        /// </summary>
        /// <returns></returns>
        void WriteInputBuffer(string line);
    }
}