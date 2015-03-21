namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Management.Automation.Host;
    using System.Management.Automation.Runspaces;
    using Autofac;
    using Microsoft.AspNet.SignalR;
    using Microsoft.AspNet.SignalR.Infrastructure;
    using Orchard;
    using Proligence.PowerShell.Provider.Console.Host;
    using Proligence.PowerShell.Provider.Console.UI;

    /// <summary>
    /// Manages the sessions of the PowerShell engine hosted in the Orchard application.
    /// </summary>
    public class PsSessionManager : IPsSessionManager
    {
        protected static readonly ConcurrentDictionary<string, IPsSession> Sessions
            = new ConcurrentDictionary<string, IPsSession>();

        private readonly IConnectionManager connectionManager;
        private readonly IComponentContext componentContext;
        private readonly OrchardPsSnapIn snapIn;

        public PsSessionManager(
            IConnectionManager connectionManager,
            IWorkContextAccessor workContextAccessor,
            IComponentContext componentContext)
        {
            this.connectionManager = connectionManager;
            this.componentContext = componentContext;
            this.snapIn = new OrchardPsSnapIn(workContextAccessor);
        }

        /// <summary>
        /// Creates a new session.
        /// </summary>
        /// <returns>The object which represents the session.</returns>
        public IPsSession NewSession(string connectionId)
        {
            lock (this.snapIn)
            {
                this.snapIn.Initialize();
            }

            RunspaceConfiguration configuration;
            try
            {
                configuration = RunspaceConfiguration.Create();

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (ProviderConfigurationEntry provider in this.snapIn.Providers) 
                {
                    if (configuration.Providers.Any(p => p.Name == provider.Name))
                    {
                        continue;
                    }

                    configuration.Providers.Append(provider);
                }

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (CmdletConfigurationEntry cmdlet in this.snapIn.Cmdlets)
                {
                    if (configuration.Cmdlets.Any(c => c.Name == cmdlet.Name))
                    {
                        continue;
                    }

                    configuration.Cmdlets.Append(cmdlet);
                }

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (FormatConfigurationEntry format in this.snapIn.Formats)
                {
                    if (configuration.Formats.Any(f => f.Name == format.Name))
                    {
                        continue;
                    }

                    configuration.Formats.Append(format);
                }

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (TypeConfigurationEntry type in this.snapIn.Types)
                {
                    if (configuration.Types.Any(t => t.Name == type.Name))
                    {
                        continue;
                    }

                    configuration.Types.Append(type);
                }

                configuration.InitializationScripts.Append(
                    new ScriptConfigurationEntry(
                        "NavigateToOrchardDrive",
                        "if (Test-Path Orchard:) { Set-Location Orchard: }"));

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (KeyValuePair<string, string> alias in this.snapIn.Aliases)
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

            var ctx = this.connectionManager.GetConnectionContext<CommandStreamConnection>();
            var consoleHost = new ConsoleHost(this.componentContext);
            var session = new PsSession(consoleHost, configuration, this.componentContext, connectionId);

            // Path is not available at this point
            session.Sender = data => ctx.Connection.Send(connectionId, data).Wait();

            session.Initialize();
            consoleHost.AttachToSession(session);

            session.Sender = data => 
            {
                data.Path = data.Path ?? session.Path + "> ";
                ctx.Connection.Send(connectionId, data).Wait();
            };

            Sessions.AddOrUpdate(
                connectionId, 
                session, 
                (s, psSession) => 
                { 
                    psSession.Dispose();
                    return session;
                });

            ctx.Connection.Send(
                connectionId, 
                new OutputData 
                {
                    Path = session.Path
                }).Wait();

            DisplayWelcomeBanner(session);

            return session;
        }

        /// <summary>
        /// Retrieves PS session for a given connection ID.
        /// </summary>
        /// <param name="connectionId">Connection ID.</param>
        /// <returns>Associated session, or null if not found.</returns>
        public IPsSession GetSession(string connectionId)
        {
            IPsSession session;
            return Sessions.TryGetValue(connectionId, out session) ? session : null;
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

            var key = Sessions
                .Where(s => s.Value == session)
                .Select(s => s.Key)
                .FirstOrDefault();

            if (key != null) 
            {
                Sessions.TryRemove(key, out session);
            }

            session.Dispose();
        }

        public void CloseSession(string connectionId) 
        {
            IPsSession session;
            if (Sessions.TryRemove(connectionId, out session))
            {
                session.Dispose();
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines")]
        private static void DisplayWelcomeBanner(IPsSession session)
        {
            PSHostUserInterface ui = session.ConsoleHost.UI;

            ui.WriteLine(ConsoleColor.Yellow, ConsoleColor.Blue, @" 
                                      ``..`                  
                               .+sdmMMMMMMMNdy+-            
                            -smMdo/-.`  `.-/sdMMNy:         
                          :dMh/`              `/hMMm/       
                        `hMh.  oso+-`            -hMMd.     
                       .mM+    MMMMMMy.     `:osy: +MMN-    
                      `mM/     dMMMMMMN-  :dMMMMM+  /MMN.   
                      sMh      .dMMMMMMh +MMMMMMM.   hMMh   
                      NM/        /yNMMMm mMMMMMN/    /MMM`  
                     `MM:        `.-:::: dmmhs/`     :MMM-  
                      NM+     .odMMMMMNy`:+-         +MMM`  
                      sMN`   oMMMMMMMMMo  -dNh:     `NMMd   
                      `NMh` -mMMMMMMms.     /NMh`  `hMMN-   
                       -NMm-  `.--.`         .NMd -mMMN:    
                        .dMMy-                sMMdMMMm-     
                          /mMMdo-            -dMMMMN+       
                            :yNMMMmhsooooshmMMMMMh/         
                               -oymMMMMMMMMMNMMm:           
                                    `.---.` `:- ");

            ui.WriteLine();
            ui.WriteLine();
            ui.WriteLine();
            ui.WriteLine(ConsoleColor.Yellow, ConsoleColor.Black, "                         Welcome to Orchard PowerShell!                        ");
            ui.WriteLine();
            ui.WriteLine();
            ui.WriteLine(
                "To get a list of all Orchard-related cmdlets, type Get-OrchardPsCommand -All.");
            ui.WriteLine(
                "To get a list of all supported cmdlets for the current location type Get-OrchardPsCommand.");
            ui.WriteLine(
                "To get help about a specific cmdlet, type Get-Help CommandName.");
            ui.WriteLine(
                "To get more help about the Orchard PowerShell provider, type Get-Help Orchard.");
            ui.WriteLine();
        }
    }
}