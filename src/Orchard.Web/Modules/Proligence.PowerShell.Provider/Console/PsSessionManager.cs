using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Infrastructure;
using Proligence.PowerShell.Provider.Console.Host;
using Proligence.PowerShell.Provider.Console.UI;

namespace Proligence.PowerShell.Provider.Console
{
    /// <summary>
    /// Manages the sessions of the PowerShell engine hosted in the Orchard application.
    /// </summary>
    public class PsSessionManager : IPsSessionManager
    {
        private readonly IPsHost host;
        private readonly IConnectionManager connectionManager;
        protected static readonly ConcurrentDictionary<string, IPsSession> sessions = new ConcurrentDictionary<string, IPsSession>();

        public PsSessionManager(IPsHost host, IConnectionManager connectionManager)
        {
            this.host = host;
            this.connectionManager = connectionManager;
        }

        /// <summary>
        /// Creates a new session.
        /// </summary>
        /// <returns>The object which represents the session.</returns>
        public IPsSession NewSession(string connectionId)
        {
            if (!this.host.IsInitialized)
            {
                this.host.Initialize();
            }

            RunspaceConfiguration configuration;
            try
            {
                configuration = RunspaceConfiguration.Create();

                foreach (ProviderConfigurationEntry provider in this.host.SnapIn.Providers)
                {
                    configuration.Providers.Append(provider);
                }

                foreach (CmdletConfigurationEntry cmdlet in this.host.SnapIn.Cmdlets)
                {
                    configuration.Cmdlets.Append(cmdlet);
                }

                foreach (FormatConfigurationEntry format in this.host.SnapIn.Formats)
                {
                    configuration.Formats.Append(format);
                }

                configuration.InitializationScripts.Append(
                    new ScriptConfigurationEntry(
                        "NavigateToOrchardDrive",
                        "if (Test-Path Orchard:) { Set-Location Orchard: }"));

                foreach (KeyValuePair<string, string> alias in this.host.SnapIn.Aliases)
                {
                    configuration.InitializationScripts.Append(
                        new ScriptConfigurationEntry(
                            "Alias-" + alias.Key,
                            "New-Alias '" + alias.Key + "' " + alias.Value));
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create runspace configuration. " + ex.Message, ex);
            }

            var ctx = connectionManager.GetConnectionContext<CommandStreamConnection>();

            var consoleHost = new ConsoleHost(configuration);
            var session = new PsSession(
                consoleHost, 
                connectionId, data => ConnectionExtensions.Send(ctx.Connection, connectionId, data).Wait());

            consoleHost.Initialize(session);

            sessions.GetOrAdd(connectionId, session);
            
            return session;
        }

        /// <summary>
        /// Retrieves PS session for a given connection ID.
        /// </summary>
        /// <param name="connectionId">Connection ID.</param>
        /// <returns>Associated session, or null if not found.</returns>
        public IPsSession GetSession(string connectionId) {
            IPsSession session;
            return sessions.TryGetValue(connectionId, out session) ? session : null;
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

            var key = sessions
                .Where(s => s.Value == session)
                .Select(s => s.Key)
                .FirstOrDefault();

            if (key != null) 
            {
                sessions.TryRemove(key, out session);
            }

            session.Dispose();
        }

        public void CloseSession(string connectionId) 
        {
            IPsSession session;
            if (sessions.TryRemove(connectionId, out session))
            {
                session.Dispose();
            }
        }
    }
}