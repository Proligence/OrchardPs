using System.Collections.Generic;

namespace Orchard.Management.PsProvider.Vfs {
    public interface IOrchardVfs {
        RootVfsNode Root { get; }
        void Initialize(IEnumerable<IPsNavigationProvider> navigationProviders);
        OrchardVfsNode NavigatePath(string path);
    }
}