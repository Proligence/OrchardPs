// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandAgent.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents
{
    using System;
    using System.Collections.Generic;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Commands.Items;

    /// <summary>
    /// Defines the interface for the legacy commands agent.
    /// </summary>
    [Agent(typeof(CommandAgent))]
    public interface ICommandAgent : IAgent
    {
        /// <summary>
        /// Gets all legacy commands which are available for the specified Orchard site.
        /// </summary>
        /// <param name="site">The name of the site.</param>
        /// <returns>
        /// An array of <see cref="OrchardCommand"/> objects which represent the Orchard commands which are available
        /// at the specified site.
        /// </returns>
        OrchardCommand[] GetCommands(string site);

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
        string ExecuteCommand(
            string siteName,
            string[] args,
            Dictionary<string, string> switches,
            bool directConsole);
    }
}