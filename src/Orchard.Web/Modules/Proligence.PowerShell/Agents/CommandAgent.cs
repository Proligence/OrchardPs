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
    using Orchard.Commands;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Commands.Items;

    /// <summary>
    /// Implements the agent which exposes legacy Orchard commands.
    /// </summary>
    public class CommandAgent : AgentBase, ICommandAgent
    {
        /// <summary>
        /// Gets all legacy commands which are available for the specified Orchard tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>
        /// An array of <see cref="OrchardCommand"/> objects which represent the Orchard commands which are available
        /// at the specified tenant.
        /// </returns>
        public OrchardCommand[] GetCommands(string tenant) 
        {
            var commandManager = this.Resolve<ICommandManager>(tenant);
            IEnumerable<CommandDescriptor> commandDescriptors = commandManager.GetCommandDescriptors();
            IEnumerable<OrchardCommand> commands = commandDescriptors.Select(
                command => new OrchardCommand 
                {
                    CommandName = command.Name,
                    HelpText = command.HelpText,
                    TenantName = tenant
                });

            return commands.ToArray();
        }

        /// <summary>
        /// Executes the specified legacy command.
        /// </summary>
        /// <param name="tenantName">The name of the tenant on which the command will be executed.</param>
        /// <param name="args">Command name and arguments.</param>
        /// <param name="switches">Command switches.</param>
        /// <param name="directConsole">
        /// <c>true</c> to force orchard output directly into <see cref="Console.Out"/>; otherwise, <c>false</c>.
        /// </param>
        /// <returns>The command's output.</returns>
        public string ExecuteCommand(
            string tenantName,
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
                    agent.RunCommand(Console.In, Console.Out, tenantName, args, switches);
                    return null;
                }
                else
                {
                    using (TextWriter writer = new StringWriter(CultureInfo.CurrentCulture))
                    {
                        agent.RunCommand(Console.In, writer, tenantName, args, switches);
                        return writer.ToString();
                    }
                }
            }
            finally 
            {
                agent.StopHost(Console.In, Console.Out);
            }
        }
    }
}