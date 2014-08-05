﻿namespace Proligence.PowerShell.Modules.NavigationProviders 
{
    using Orchard.Environment.Extensions;
    using Proligence.PowerShell.Modules.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="FeaturesNode"/> tenant node to the Orchard VFS.
    /// </summary>
    public class FeaturesPsNavigationProvider : PsNavigationProvider 
    {
        private readonly IExtensionManager extensionManager;

        public FeaturesPsNavigationProvider(IExtensionManager extensionManager)
            : base(NodeType.Tenant)
        {
            this.extensionManager = extensionManager;
        }

        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize()
        {
            this.Node = new FeaturesNode(this.Vfs, this.extensionManager);
        }
    }
}