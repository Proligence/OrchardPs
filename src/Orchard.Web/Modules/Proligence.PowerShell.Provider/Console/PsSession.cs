namespace Proligence.PowerShell.Provider.Console
{
    using System;
    using System.Collections.Concurrent;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using Proligence.PowerShell.Provider.Console.Host;
    using Proligence.PowerShell.Provider.Console.UI;

    /// <summary>
    /// Represents a PowerShell user session.
    /// </summary>
    public class PsSession : IPsSession 
    {
        private readonly ConcurrentQueue<string> queue;

        public PsSession(ConsoleHost consoleHost, string connectionId)
        {
            this.ConsoleHost = consoleHost;
            this.ConnectionId = connectionId;

            this.queue = new ConcurrentQueue<string>();
        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        /// <summary>
        /// Gets the console host which provides input/output to the PowerShell engine.
        /// </summary>
        public ConsoleHost ConsoleHost { get; private set; }

        /// <summary>
        /// Gets the PowerShell runspace associated with the session.
        /// </summary>
        public Runspace Runspace
        {
            get { return this.ConsoleHost.Runspace; }
        }

        /// <summary>
        /// Gets the current session's runspace path details.
        /// </summary>
        public PathIntrinsics PathIntrinsics
        {
            get 
            {
                return this.Runspace.SessionStateProxy.Path;
            }
        }

        /// <summary>
        /// Gets the current session's runspace absolute path.
        /// </summary>
        public string Path
        {
            get
            {
                return this.PathIntrinsics.CurrentLocation.ToString();
            }
        }

        /// <summary>
        ///  Delegate used for sending messages up to the user console.
        /// </summary>
        public Action<OutputData> Sender { get; internal set; }

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
            if (this.queue.TryDequeue(out result)) {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Writes a line to the input buffer.
        /// </summary>
        /// <returns></returns>
        public void WriteInputBuffer(string line) {
            this.queue.Enqueue(line);
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

        protected virtual void OnDataReceived(DataReceivedEventArgs e)
        {
            EventHandler<DataReceivedEventArgs> handler = DataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}