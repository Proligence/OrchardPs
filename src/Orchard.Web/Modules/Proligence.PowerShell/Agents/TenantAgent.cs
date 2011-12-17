using System.Collections.Generic;
using System.Linq;
using Autofac;
using Orchard.Environment.Configuration;
using Orchard.Management.PsProvider.Agents;
using Orchard.PowerShell.Sites.Items;

namespace Orchard.PowerShell.Agents {
    public class TenantAgent : AgentBase {
        public OrchardSite[] GetSites() {
            IShellSettingsManager tenantManager = HostContainer.Resolve<IShellSettingsManager>();
            IEnumerable<ShellSettings> settings = tenantManager.LoadSettings();
            IEnumerable<OrchardSite> sites = settings.Select(
                s => new OrchardSite {
                    Name = s.Name,
                    State = s.State.CurrentState,
                    DataConnectionString = s.DataConnectionString,
                    DataProvider = s.DataProvider,
                    DataTablePrefix = s.DataTablePrefix,
                    EncryptionAlgorithm = s.EncryptionAlgorithm,
                    EncryptionKey = s.EncryptionKey,
                    HashAlgorithm = s.HashAlgorithm,
                    HashKey = s.HashKey,
                    RequestUrlHost = s.RequestUrlHost,
                    RequestUrlPrefix = s.RequestUrlPrefix
                });

            return sites.ToArray();
        }
    }
}