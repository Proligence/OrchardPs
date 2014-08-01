namespace Proligence.PowerShell.Provider.Console
{
    using System;
    using System.Collections.Concurrent;
    using System.Management.Automation.Runspaces;
    using System.Threading;
    using Proligence.PowerShell.Provider.Console.Host;
    using Proligence.PowerShell.Provider.Console.UI;

    /// <summary>
    /// Represents a PowerShell user session.
    /// </summary>
    public class PsSession : IPsSession 
    {
        private readonly ConcurrentQueue<string> queue;
        private readonly AutoResetEvent runspaceLock;

        /// <summary>
        /// Caches the current path of the session's runspace. This cached value is used if the runspace cannot be
        /// accessed because a cmdlet is being executed in it.
        /// </summary>
        private string currentPath;

        public PsSession(ConsoleHost consoleHost, string connectionId)
        {
            this.ConsoleHost = consoleHost;
            this.ConnectionId = connectionId;

            this.queue = new ConcurrentQueue<string>();
            this.runspaceLock = new AutoResetEvent(true);
            this.Runspace.StateChanged += this.OnRunspaceStateChanged;
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
        /// Gets the lock which must be aquired before acessing the session's runspace.
        /// </summary>
        public EventWaitHandle RunspaceLock
        {
            get { return this.runspaceLock; }
        }

        /// <summary>
        /// Gets the current session's runspace absolute path.
        /// </summary>
        public string Path
        {
            get
            {
                if (this.runspaceLock.WaitOne(0))
                {
                    try
                    {
                        this.currentPath = this.ConsoleHost.Runspace.SessionStateProxy.Path.CurrentLocation.ToString();
                    }
                    finally
                    {
                        this.runspaceLock.Set();
                    }
                }

                return this.currentPath ?? string.Empty;
            }
        }

        /// <summary>
        /// Delegate used for sending messages up to the user console.
        /// </summary>
        public Action<OutputData> Sender { get; internal set; }

        /// <summary>
        /// SignalR connection identifier for this particular session.
        /// </summary>
        public string ConnectionId { get; private set; }

        /// <summary>
        /// Reads line of string from input buffer. Nonblocking.
        /// </summary>
        public string ReadInputBuffer()
        {
            string result;
            if (this.queue.TryDequeue(out result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Writes a line to the input buffer.
        /// </summary>
        public void WriteInputBuffer(string line)
        {
            this.queue.Enqueue(line);
            this.OnDataReceived(new DataReceivedEventArgs());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() 
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.ConsoleHost != null)
                {
                    this.ConsoleHost.Dispose();
                }
            }
        }

        protected virtual void OnDataReceived(DataReceivedEventArgs e)
        {
            EventHandler<DataReceivedEventArgs> handler = this.DataReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void OnRunspaceStateChanged(object sender, RunspaceStateEventArgs e)
        {
            // Intialize the current path after the runspace is opened.
            if (e.RunspaceStateInfo.State == RunspaceState.Opened)
            {
                this.currentPath = this.Runspace.SessionStateProxy.Path.CurrentLocation.ToString();
            }
        }
    }
}