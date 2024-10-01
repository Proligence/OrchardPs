using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions.Models;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Modules.Nodes {
    /// <summary>
    /// Implements a VFS node which represents an Orchard module.
    /// </summary>
    public class ModuleNode : ContainerNode {
        public ModuleNode(IPowerShellVfs vfs, ExtensionDescriptor module)
            : base(vfs, module.Name, GetSubNodes(vfs, module)) {
            Item = module;
        }

        private static IEnumerable<VfsNode> GetSubNodes(IPowerShellVfs vfs, ExtensionDescriptor module) {
            return module.Features.Select(f => new FeatureNode(vfs, f));
        }
    }
}