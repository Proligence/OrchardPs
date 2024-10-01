using System.Linq;
using System.Management.Automation;
using Orchard.Environment.Configuration;
using Proligence.PowerShell.Provider.Vfs.Navigation;
using Proligence.PowerShell.Provider.Vfs.Nodes;

namespace Proligence.PowerShell.Provider.Vfs.NavigationProviders {
    public class DefaultTenantPsNavigationProvider : PsNavigationProvider {
        private readonly IShellSettingsManager _shellSettingsManager;

        public DefaultTenantPsNavigationProvider(IShellSettingsManager shellSettingsManager)
            : base(NodeType.Global) {
            _shellSettingsManager = shellSettingsManager;
        }

        protected override void InitializeInternal() {
            ShellSettings tenant = _shellSettingsManager
                .LoadSettings()
                .First(t => t.Name == "Default");

            var defaultTenant = new DefaultShellSettings {
                Name = tenant.Name,
                State = tenant.State,
                DataConnectionString = tenant.DataConnectionString,
                DataProvider = tenant.DataProvider,
                DataTablePrefix = tenant.DataTablePrefix,
                EncryptionAlgorithm = tenant.EncryptionAlgorithm,
                EncryptionKey = tenant.EncryptionKey,
                HashAlgorithm = tenant.HashAlgorithm,
                HashKey = tenant.HashKey,
                Modules = tenant.Modules,
                RequestUrlHost = tenant.RequestUrlHost,
                RequestUrlPrefix = tenant.RequestUrlPrefix,
                Themes = tenant.Themes
            };

            var psobj = PSObject.AsPSObject(defaultTenant);
            psobj.Properties.Add(new PSNoteProperty("Name", "$"));
            psobj.Properties.Add(new PSNoteProperty("Description", "$ -> Tenants\\Default"));

            Node = new TenantNode(Vfs, psobj, "$");
        }
    }
}