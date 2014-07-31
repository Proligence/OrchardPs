using System;
using System.Collections.Concurrent;
using Proligence.PowerShell.Provider.Console.Host;

namespace Proligence.PowerShell.Provider.Console
{
    public class DataReceivedEventArgs : EventArgs 
    {
    }

    /// <summary>
    /// Represents a PowerShell user session.
    /// </summary>
    public class PsSession : IPsSession {
        private readonly ConcurrentQueue<string> _queue;

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        protected virtual void OnDataReceived(DataReceivedEventArgs e) {
            EventHandler<DataReceivedEventArgs> handler = DataReceived;
            if (handler != null) {
                handler(this, e);
            }
        }

        public PsSession(ConsoleHost consoleHost, string connectionId, Action<dynamic> sender) {
            Sender = sender;
            ConsoleHost = consoleHost;
            ConnectionId = connectionId;

            _queue = new ConcurrentQueue<string>();
        }

        /// <summary>
        /// Gets the console host which provides input/output to the PowerShell engine.
        /// </summary>
        public ConsoleHost ConsoleHost { get; private set; }

        /// <summary>
        ///  Delegate used for sending messages up to the user console.
        /// </summary>
        public Action<dynamic> Sender { get; private set; }

        /// <summary>
        /// SignalR connection identifier for this particular session.
        /// </summary>
        public string ConnectionId { get; private set; }

        /// <summary>
        /// Reads line of string from input buffer. Nonblocking.
        /// </summary>
        /// <returns></returns>
        public string ReadInputBuffer() {
            string result;
            if (_queue.TryDequeue(out result)) {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Writes a line to the input buffer.
        /// </summary>
        /// <returns></returns>
        public void WriteInputBuffer(string line) {
            _queue.Enqueue(line);
            OnDataReceived(new DataReceivedEventArgs());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposing) {
                return;
            }

            if (ConsoleHost != null) {
                ConsoleHost.Dispose();
            }
        }
    }
}