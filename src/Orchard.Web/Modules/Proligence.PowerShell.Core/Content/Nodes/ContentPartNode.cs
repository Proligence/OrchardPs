using Orchard.ContentManagement.MetaData.Models;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Content.Nodes {
    /// <summary>
    /// Implements a VFS node which represents an Orchard content part definition.
    /// </summary>
    [SupportedCmdlet("Edit-ContentPartDefinition")]
    [SupportedCmdlet("Add-ContentField")]
    [SupportedCmdlet("Remove-ContentField")]
    public class ContentPartNode : ObjectNode {
        public ContentPartNode(IPowerShellVfs vfs, ContentPartDefinition definition)
            : base(vfs, definition.Name, definition) {
        }
    }
}