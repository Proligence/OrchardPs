namespace Proligence.PowerShell.Modules.NavigationProviders 
{
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Modules.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="FeaturesNode"/> tenant node to the Orchard VFS.
    /// </summary>
    public class FeaturesPsNavigationProvider : PsNavigationProvider 
    {
        private readonly IModulesAgent agent;

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            this.NodeType = NodeType.Site;
            this.Node = new FeaturesNode(this.Vfs, this.agent);
        }
    }
}