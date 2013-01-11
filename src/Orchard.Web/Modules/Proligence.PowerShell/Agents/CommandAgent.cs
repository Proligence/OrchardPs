// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandAgent.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents 
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Autofac;
    using Orchard.Commands;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Commands.Items;

    /// <summary>
    /// Implements the agent which exposes legacy Orchard commands.
    /// </summary>
    public class CommandAgent : AgentBase 
    {
        /// <summary>
        /// Gets all legacy commands which are available for the specified Orchard site.
        /// </summary>
        /// <param name="site">The name of the site.</param>
        /// <returns>
        /// An array of <see cref="OrchardCommand"/> objects which represent the Orchard commands which are available
        /// at the specified site.
        /// </returns>
        public OrchardCommand[] GetCommands(string site) 
        {
            ICommandManager commandManager = this.GetCommandManager(site);
            IEnumerable<CommandDescriptor> commandDescriptors = commandManager.GetCommandDescriptors();
            IEnumerable<OrchardCommand> commands = commandDescriptors.Select(
                command => new OrchardCommand 
                {
                    CommandName = command.Name,
                    HelpText = command.HelpText,
                    SiteName = site
                });

            return commands.ToArray();
        }

        /// <summary>
        /// Executes the specified legacy command.
        /// </summary>
        /// <param name="siteName">The name of the site on which the command will be exectued.</param>
        /// <param name="args">Command name and arguments.</param>
        /// <param name="switches">Command switches.</param>
        /// <param name="directConsole">
        /// <c>true</c> to force orchard output directly into <see cref="Console.Out"/>; otherwise, <c>false</c>.
        /// </param>
        /// <returns>The command's output.</returns>
        public string ExecuteCommand(
            string siteName,
            string[] args,
            Dictionary<string, string> switches,
            bool directConsole) 
        {
            var agent = new CommandHostAgent();
            agent.StartHost(Console.In, Console.Out);
            try 
            {
                if (directConsole)
                {
                    agent.RunCommand(Console.In, Console.Out, siteName, args, switches);
                    return null;
                }
                else
                {
                    using (TextWriter writer = new StringWriter(CultureInfo.CurrentCulture))
                    {
                        agent.RunCommand(Console.In, writer, siteName, args, switches);
                        return writer.ToString();
                    }
                }
                
            }
            finally 
            {
                agent.StopHost(Console.In, Console.Out);
            }
        }

        /// <summary>
        /// Gets the <see cref="ICommandManager"/> instance for the specified site.
        /// </summary>
        /// <param name="site">The name of the site.</param>
        /// <returns>The <see cref="ICommandManager"/> instance for the specified site.</returns>
        private ICommandManager GetCommandManager(string site) 
        {
            ILifetimeScope siteContainer = this.ContainerManager.GetSiteContainer(site);
            return siteContainer.Resolve<ICommandManager>();
        }
    }
}