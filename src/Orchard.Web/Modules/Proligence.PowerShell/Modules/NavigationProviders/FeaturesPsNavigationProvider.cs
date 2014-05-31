// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeaturesPsNavigationProvider.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.NavigationProviders 
{
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Modules.Nodes;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="FeaturesNode"/> site node to the Orchard VFS.
    /// </summary>
    public class FeaturesPsNavigationProvider : PsNavigationProvider 
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
            this.Node = new FeaturesNode(this.Vfs, this.AgentManager.GetAgent<IModulesAgent>());
        }
    }
}