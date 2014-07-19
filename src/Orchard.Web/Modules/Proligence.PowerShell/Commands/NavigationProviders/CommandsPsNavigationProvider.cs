namespace Proligence.PowerShell.Commands.NavigationProviders 
{
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Commands.Nodes;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="CommandsNode"/> tenant node to the Orchard VFS.
    /// </summary>
    public class CommandsPsNavigationProvider : PsNavigationProvider 
    {
        /// <summary>
        /// Gets or sets the Orchard agent manager.
        /// </summary>
        public IAgentManager AgentManager { get; set; }

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            this.NodeType = NodeType.Site;
            this.Node = new CommandsNode(this.Vfs, this.AgentManager.GetAgent<ICommandAgent>());
        }
    }
}