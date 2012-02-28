// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandAgentProxy.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Agents 
{
    using System.Collections.Generic;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Commands.Items;

    /// <summary>
    /// Implements the proxy for the agent which exposes legacy Orchard commands.
    /// </summary>
    [Agent("Proligence.PowerShell.Agents.CommandAgent, Proligence.PowerShell, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null")]
    public class CommandAgentProxy : AgentProxy 
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
            return (OrchardCommand[])Invoke("GetCommands", site);
        }

        /// <summary>
        /// Executes the specified legacy command.
        /// </summary>
        /// <param name="siteName">The name of the site on which the command will be exectued.</param>
        /// <param name="args">Command name and arguments.</param>
        /// <param name="switches">Command switches.</param>
        public void ExecuteCommand(string siteName, string[] args, Dictionary<string, string> switches) 
        {
            Invoke("ExecuteCommand", siteName, args, switches);
        }
    }
}