namespace Proligence.PowerShell.Modules.Nodes
{
    using System.Diagnostics.CodeAnalysis;

    using Orchard.Environment.Extensions.Models;

    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

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
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        public FeatureNode(IPowerShellVfs vfs, FeatureDescriptor feature)
            : base(vfs, feature.Name, feature)
        {
        }
    }
}