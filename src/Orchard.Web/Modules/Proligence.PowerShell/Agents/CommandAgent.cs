using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Orchard.Commands;
using Orchard.Management.PsProvider.Agents;
using Proligence.PowerShell.Commands.Items;

namespace Proligence.PowerShell.Agents {
    public class CommandAgent : AgentBase {
        public OrchardCommand[] GetCommands(string site) {
            ICommandManager commandManager = GetCommandManager(site);
            IEnumerable<CommandDescriptor> commandDescriptors = commandManager.GetCommandDescriptors();
            IEnumerable<OrchardCommand> commands = commandDescriptors.Select(
                command => new OrchardCommand {
                    CommandName = command.Name,
                    HelpText = command.HelpText,
                    SiteName = site
                });

            return commands.ToArray();
        }

        public void ExecuteCommand(string siteName, string[] args, Dictionary<string, string> switches) {
            var agent = new CommandHostAgent();
            agent.StartHost(Console.In, Console.Out);
            try {
                agent.RunCommand(Console.In, Console.Out, siteName, args, switches);
            }
            finally {
                agent.StopHost(Console.In, Console.Out);
            }
        }

        private ICommandManager GetCommandManager(string site) {
            ILifetimeScope siteContainer = ContainerManager.GetSiteContainer(site);
            return siteContainer.Resolve<ICommandManager>();
        }
    }
}