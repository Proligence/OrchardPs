namespace Proligence.PowerShell.Host
{
    using System;
    using System.Collections.Generic;
    using System.Management.Automation.Runspaces;
    using Orchard.Management.PsProvider.Console;

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

            var consoleHost = new ConsoleHost(configuration);
            var session = new PsSession(consoleHost);
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