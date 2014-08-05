namespace Proligence.PowerShell.Modules.Nodes 
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Orchard.Environment.Extensions.Models;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard module.
    /// </summary>
    public class ModuleNode : ContainerNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="module">The object of the legacy Orchard module represented by the node.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        public ModuleNode(IPowerShellVfs vfs, ExtensionDescriptor module)
            : base(vfs, module.Name, GetSubNodes(vfs, module))
        {
            this.Item = module;
        }

        /// <summary>
        /// Gets the subnodes of the node.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="module">The object which represents the Orchard module.</param>
        /// <returns>A sequence of VFS nodes.</returns>
        private static IEnumerable<VfsNode> GetSubNodes(IPowerShellVfs vfs, ExtensionDescriptor module)
        {
            return module.Features.Select(f => new FeatureNode(vfs, f));
        }
    }
}