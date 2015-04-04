namespace Proligence.PowerShell.Core.Content.Nodes
{
    using Orchard.ContentManagement.MetaData.Models;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard content part definition.
    /// </summary>
    [SupportedCmdlet("Edit-ContentPartDefinition")]
    public class ContentPartNode : ObjectNode
    {
        public ContentPartNode(IPowerShellVfs vfs, ContentPartDefinition definition)
            : base(vfs, definition.Name, definition)
        {
        }
    }
}