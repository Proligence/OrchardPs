namespace Proligence.PowerShell.Tenants.NavigationProviders 
{
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Tenants.Nodes;

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