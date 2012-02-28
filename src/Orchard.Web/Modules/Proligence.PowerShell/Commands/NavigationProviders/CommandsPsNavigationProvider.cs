// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandsPsNavigationProvider.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Commands.NavigationProviders 
{
    using Orchard.Management.PsProvider.Vfs;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Commands.Nodes;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="CommandsNode"/> site node to the Orchard VFS.
    /// </summary>
    public class CommandsPsNavigationProvider : PsNavigationProvider 
    {
        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            NodeType = NodeType.Site;
            Node = new CommandsNode(Vfs, AgentManager.GetAgent<CommandAgentProxy>());
        }
    }
}