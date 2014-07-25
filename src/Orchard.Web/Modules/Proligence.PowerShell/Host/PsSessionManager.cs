namespace Proligence.PowerShell.Host
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages the sessions of the PowerShell engine hosted in the Orchard application.
    /// </summary>
    public class PsSessionManager : IPsSessionManager
    {
        private readonly IPsHost host;
        private readonly IList<IPsSession> sessions;

        public PsSessionManager(IPsHost host)
        {
            this.host = host;
            this.sessions = new List<IPsSession>();
        }

        /// <summary>
        /// Creates a new session.
        /// </summary>
        /// <returns>The object which represents the session.</returns>
        public IPsSession NewSession()
        {
            if (!this.host.IsInitialized)
            {
                this.host.Initialize();
            }

            var session = new PsSession();
            this.sessions.Add(session);
            
            return session;
        }

        /// <summary>
        /// Closes the specified session.
        /// </summary>
        /// <param name="session">The session to close.</param>
        public void CloseSession(IPsSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }

            this.sessions.Remove(session);
            session.Dispose();
        }
    }
}