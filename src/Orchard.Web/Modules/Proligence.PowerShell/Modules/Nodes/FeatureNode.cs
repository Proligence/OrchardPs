namespace Proligence.PowerShell.Modules.Nodes
{
    using Orchard.Environment.Extensions.Models;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard feature.
    /// </summary>
    [SupportedCmdlet("Enable-OrchardFeature")]
    [SupportedCmdlet("Disable-OrchardFeature")]
    public class FeatureNode : ObjectNode
    {
        public FeatureNode(IPowerShellVfs vfs, FeatureDescriptor feature)
            : base(vfs, feature.Name ?? feature.Id, feature)
        {
        }
    }
}