﻿using Orchard;

namespace Proligence.PowerShell.Provider {
    /// <summary>
    /// Manages the sessions of the PowerShell engine hosted in the Orchard application.
    /// </summary>
    public interface IPsSessionManager : ISingletonDependency {
        /// <summary>
        /// Creates a new session.
        /// </summary>
        /// <param name="connectionId">The identifier of the connection.</param>
        /// <param name="connection">The connection proxy implementation.</param>
        /// <returns>The object which represents the session.</returns>
        IPsSession NewSession(string connectionId, IConsoleConnection connection);

        /// <summary>
        /// Retrieves PS session for a given connection ID.
        /// </summary>
        /// <param name="connectionId">Connection ID.</param>
        /// <returns>Associated session, or null if not found.</returns>
        IPsSession GetSession(string connectionId);

        /// <summary>
        /// Closes the specified session.
        /// </summary>
        /// <param name="session">The session to close.</param>
        void CloseSession(IPsSession session);

        /// <summary>
        /// Closes the specified session with given connection ID.
        /// </summary>
        /// <param name="connectionId">Id of the connection connected with session to be closed.</param>
        void CloseSession(string connectionId);
    }
}