namespace Proligence.PowerShell.Content.NavigationProviders 
{
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Content.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="ContentNode"/> tenant node to the Orchard VFS.
    /// </summary>
    public class ContentPsNavigationProvider : PsNavigationProvider 
    {
        private readonly IContentAgent agent;

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            this.NodeType = NodeType.Site;
            this.Node = new ContentNode(this.Vfs, this.agent);
        }
    }
}