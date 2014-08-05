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

        public override void Initialize()
        {
            this.Node = new TenantConfigurationNode(this.Vfs);
        }
    }
}