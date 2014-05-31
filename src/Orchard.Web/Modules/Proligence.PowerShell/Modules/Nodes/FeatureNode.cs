// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.Nodes
{
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard feature.
    /// </summary>
    [SupportedCmdlet("Enable-OrchardFeature")]
    [SupportedCmdlet("Disable-OrchardFeature")]
    public class FeatureNode : ObjectNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="feature">The object of the Orchard feature represented by the node.</param>
        public FeatureNode(IPowerShellVfs vfs, OrchardFeature feature)
            : base(vfs, feature.Name, feature)
        {
        }
    }
}