﻿namespace Proligence.PowerShell.Provider.Vfs.NavigationProviders 
{
    using Orchard.Environment.Configuration;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Provider.Vfs.Nodes;

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