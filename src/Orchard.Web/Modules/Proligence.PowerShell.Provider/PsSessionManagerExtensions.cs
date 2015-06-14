namespace Proligence.PowerShell.Provider
{
    using System;

    public static class PsSessionManagerExtensions
    {
        public static IPsSession CreateSession(this IPsSessionManager sessionManager)
        {
            return sessionManager.NewSession(Guid.NewGuid().ToString(), new BufferConsoleConnection());
        }

        public static IPsSession CreateSession(this IPsSessionManager sessionManager, out BufferConsoleConnection connection)
        {
            connection = new BufferConsoleConnection();
            return sessionManager.NewSession(Guid.NewGuid().ToString(), connection);
        }
    }
}