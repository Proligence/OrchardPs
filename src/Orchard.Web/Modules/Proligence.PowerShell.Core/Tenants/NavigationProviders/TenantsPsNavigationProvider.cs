namespace Proligence.PowerShell.Core.Tenants.NavigationProviders 
{
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Core.Tenants.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    
    public class TenantsPsNavigationProvider : PsNavigationProvider
    {
        private readonly IShellSettingsManager manager;

        public TenantsPsNavigationProvider(IShellSettingsManager manager)
            : base(NodeType.Global)
        {
            this.manager = manager;
        }

        protected override void InitializeInternal()
        {
            this.Node = new TenantsNode(this.Vfs, this.manager);
        }
    }
}