using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using Autofac;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Validation;
using Proligence.PowerShell.Provider.Console.Host;
using Proligence.PowerShell.Provider.Console.UI;
using Proligence.PowerShell.Provider.Internal;

namespace Proligence.PowerShell.Provider {
    /// <summary>
    /// Manages the sessions of the PowerShell engine hosted in the Orchard application.
    /// </summary>
    public class PsSessionManager : IPsSessionManager {
        protected static readonly ConcurrentDictionary<string, IPsSession> Sessions
            = new ConcurrentDictionary<string, IPsSession>();

        private readonly IComponentContext _componentContext;
        private readonly OrchardPsSnapIn _snapIn;

        public PsSessionManager(
            IComponentContext componentContext,
            IPsFileSearcher fileSearcher,
            IExtensionManager extensionManager,
            ShellDescriptor shellDescriptor) {
            _componentContext = componentContext;
            _snapIn = new OrchardPsSnapIn(fileSearcher, extensionManager, shellDescriptor);
        }

        public IPsSession NewSession(string connectionId, IConsoleConnection connection) {
            lock (_snapIn) {
                _snapIn.Initialize();
            }

            RunspaceConfiguration configuration;
            try {
                configuration = RunspaceConfiguration.Create();

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (ProviderConfigurationEntry provider in _snapIn.Providers) {
                    if (configuration.Providers.Any(p => p.Name == provider.Name)) {
                        continue;
                    }

                    configuration.Providers.Append(provider);
                }

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (CmdletConfigurationEntry cmdlet in _snapIn.Cmdlets) {
                    if (configuration.Cmdlets.Any(c => c.Name == cmdlet.Name)) {
                        continue;
                    }

                    configuration.Cmdlets.Append(cmdlet);
                }

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (FormatConfigurationEntry format in _snapIn.Formats) {
                    if (configuration.Formats.Any(f => f.Name == format.Name)) {
                        continue;
                    }

                    configuration.Formats.Append(format);
                }

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (TypeConfigurationEntry type in _snapIn.Types) {
                    if (configuration.Types.Any(t => t.Name == type.Name)) {
                        continue;
                    }

                    configuration.Types.Append(type);
                }

                configuration.InitializationScripts.Append(
                    new ScriptConfigurationEntry(
                        "NavigateToOrchardDrive",
                        "if (Test-Path Orchard:) { Set-Location Orchard: }"));

                // ReSharper disable once InconsistentlySynchronizedField
                foreach (KeyValuePair<string, string> alias in _snapIn.Aliases) {
                    configuration.InitializationScripts.Append(
                        new ScriptConfigurationEntry(
                            "Alias-" + alias.Key,
                            "New-Alias '" + alias.Key + "' " + alias.Value));
                }
            }
            catch (Exception ex) {
                throw new InvalidOperationException("Failed to create runspace configuration. " + ex.Message, ex);
            }

            connection.Initialize();
            var consoleHost = new ConsoleHost(_componentContext);
            var session = new PsSession(consoleHost, configuration, _componentContext, connectionId);

            // Path is not available at this point
            session.Sender = data => connection.Send(connectionId, data);

            session.Initialize();
            consoleHost.AttachToSession(session);
            session.Path = session.Runspace.SessionStateProxy.Path.CurrentLocation.ToString();

            session.Sender = data => {
                data.Prompt = data.Prompt ?? session.Path + "> ";
                connection.Send(connectionId, data);
            };

            Sessions.AddOrUpdate(
                connectionId,
                session,
                (s, psSession) => {
                    psSession.Dispose();
                    return session;
                });

            var outputData = new OutputData {Prompt = session.Path + "> "};
            connection.Send(connectionId, outputData);

            DisplayWelcomeBanner(session);

            return session;
        }

        /// <summary>
        /// Retrieves PS session for a given connection ID.
        /// </summary>
        /// <param name="connectionId">Connection ID.</param>
        /// <returns>Associated session, or null if not found.</returns>
        public IPsSession GetSession(string connectionId) {
            IPsSession session;
            return Sessions.TryGetValue(connectionId, out session)
                ? session
                : null;
        }

        /// <summary>
        /// Closes the specified session.
        /// </summary>
        /// <param name="session">The session to close.</param>
        public void CloseSession(IPsSession session) {
            Argument.ThrowIfNull(session, "session");

            var key = Sessions
                .Where(s => s.Value == session)
                .Select(s => s.Key)
                .FirstOrDefault();

            if (key != null) {
                Sessions.TryRemove(key, out session);
            }

            // NOTE: Sometimes the session may end up collected by the GC, when the host is shut down from the
            // AppDomain of OrchardPs.exe. In that case, there is no need to dispose it.
            if (session != null) {
                session.Dispose();
            }
        }

        public void CloseSession(string connectionId) {
            IPsSession session;
            if (Sessions.TryRemove(connectionId, out session)) {
                session.Dispose();
            }
        }

        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1116:SplitParametersMustStartOnLineAfterDeclaration")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1117:ParametersMustBeOnSameLineOrSeparateLines")]
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1118:ParameterMustNotSpanMultipleLines")]
        private static void DisplayWelcomeBanner(IPsSession session) {
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

            ui.WriteLine(
                ConsoleColor.Yellow,
                ConsoleColor.Black,
                "                         Welcome to Orchard PowerShell!                        ");

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