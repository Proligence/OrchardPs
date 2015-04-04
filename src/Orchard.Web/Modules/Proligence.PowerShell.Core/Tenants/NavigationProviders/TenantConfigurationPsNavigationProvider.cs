namespace Proligence.PowerShell.Core.Tenants.NavigationProviders 
{
    using Proligence.PowerShell.Core.Tenants.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    
    public class TenantConfigurationPsNavigationProvider : PsNavigationProvider
    {
        public TenantConfigurationPsNavigationProvider()
            : base(NodeType.Tenant)
        {
        }

        protected override void InitializeInternal()
        {
            this.Node = new TenantConfigurationNode(this.Vfs);
        }
    }
}