namespace Proligence.PowerShell.Tenants.NavigationProviders 
{
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Tenants.Nodes;

    public class TenantsPsNavigationProvider : PsNavigationProvider
    {
        private readonly IShellSettingsManager manager;

        public TenantsPsNavigationProvider(IShellSettingsManager manager)
            : base(NodeType.Global)
        {
            this.manager = manager;
        }

        public override void Initialize()
        {
            this.Node = new TenantsNode(this.Vfs, this.manager);
        }
    }
}