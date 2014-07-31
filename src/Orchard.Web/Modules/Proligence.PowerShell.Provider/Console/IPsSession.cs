using System;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Proligence.PowerShell.Provider.Console.Host;

namespace Proligence.PowerShell.Provider.Console
{
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
        ///  Delegate used for sending messages up to the user console.
        /// </summary>
        Action<dynamic> Sender { get; }

        /// <summary>
        /// SignalR connection identifier for this particular session.
        /// </summary>
        string ConnectionId { get; }

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