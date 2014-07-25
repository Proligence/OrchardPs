namespace Proligence.PowerShell.Host
{
    using Orchard;

    /// <summary>
    /// Manages the sessions of the PowerShell engine hosted in the Orchard application.
    /// </summary>
    public interface IPsSessionManager : IDependency
    {
        /// <summary>
        /// Creates a new session.
        /// </summary>
        /// <returns>The object which represents the session.</returns>
        IPsSession NewSession();

        /// <summary>
        /// Closes the specified session.
        /// </summary>
        /// <param name="session">The session to close.</param>
        void CloseSession(IPsSession session);
    }
}