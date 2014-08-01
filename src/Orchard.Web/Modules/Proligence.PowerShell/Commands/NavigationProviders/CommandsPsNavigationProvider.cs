namespace Proligence.PowerShell.Commands.NavigationProviders 
{
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Commands.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="CommandsNode"/> tenant node to the Orchard VFS.
    /// </summary>
    public class CommandsPsNavigationProvider : PsNavigationProvider 
    {
        private readonly ICommandAgent agent;

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            this.NodeType = NodeType.Site;
            this.Node = new CommandsNode(this.Vfs, this.agent);
        }
    }
}