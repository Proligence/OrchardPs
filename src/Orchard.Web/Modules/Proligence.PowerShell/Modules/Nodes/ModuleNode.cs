// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModuleNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

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
        public ModuleNode(IPowerShellVfs vfs, OrchardModule module)
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
        private static IEnumerable<VfsNode> GetSubNodes(IPowerShellVfs vfs, OrchardModule module)
        {
            return module.Features.Select(f => new FeatureNode(vfs, f));
        }
    }
}