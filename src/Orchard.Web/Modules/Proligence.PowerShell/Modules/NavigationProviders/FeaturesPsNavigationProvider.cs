namespace Proligence.PowerShell.Modules.NavigationProviders 
{
    using Orchard.Environment.Extensions;
    using Proligence.PowerShell.Modules.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class FeaturesPsNavigationProvider : PsNavigationProvider 
    {
        private readonly IExtensionManager extensionManager;

        public FeaturesPsNavigationProvider(IExtensionManager extensionManager)
            : base(NodeType.Tenant)
        {
            this.extensionManager = extensionManager;
        }

        protected override void InitializeInternal()
        {
            this.Node = new FeaturesNode(this.Vfs, this.extensionManager);
        }
    }
}