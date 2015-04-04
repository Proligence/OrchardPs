namespace Proligence.PowerShell.Core.Modules.NavigationProviders 
{
    using Orchard.Environment.Extensions;
    using Proligence.PowerShell.Core.Modules.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class ModulesPsNavigationProvider : PsNavigationProvider 
    {
        private readonly IExtensionManager extensionManager;

        public ModulesPsNavigationProvider(IExtensionManager extensionManager)
            : base(NodeType.Tenant)
        {
            this.extensionManager = extensionManager;
        }

        protected override void InitializeInternal()
        {
            this.Node = new ModulesNode(this.Vfs, this.extensionManager);
        }
    }
}