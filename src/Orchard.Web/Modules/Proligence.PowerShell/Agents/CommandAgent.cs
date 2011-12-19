using System.Collections.Generic;
using System.Linq;
using Autofac;
using Orchard.Commands;
using Orchard.Management.PsProvider.Agents;
using Proligence.PowerShell.Commands.Items;

namespace Proligence.PowerShell.Agents {
    public class CommandAgent : AgentBase {
        public OrchardCommand[] GetCommands(string site) {
            ILifetimeScope siteContainer = ContainerManager.GetSiteContainer(site);
            ICommandManager commandManager = siteContainer.Resolve<ICommandManager>();
            IEnumerable<CommandDescriptor> commandDescriptors = commandManager.GetCommandDescriptors();
            IEnumerable<OrchardCommand> commands = commandDescriptors.Select(
                command => new OrchardCommand {
                    CommandName = command.Name,
                    HelpText = command.HelpText,
                    SiteName = site
                });

            return commands.ToArray();
        }
    }
}