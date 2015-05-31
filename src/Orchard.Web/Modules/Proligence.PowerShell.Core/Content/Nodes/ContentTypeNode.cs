namespace Proligence.PowerShell.Core.Content.Nodes
{
    using Orchard.ContentManagement.MetaData.Models;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard content type definition.
    /// </summary>
    [SupportedCmdlet("Edit-ContentType")]
    [SupportedCmdlet("Get-ContentPart")]
    [SupportedCmdlet("Add-ContentPart")]
    [SupportedCmdlet("Remove-ContentPart")]
    [SupportedCmdlet("New-ContentItem")]
    public class ContentTypeNode : ObjectNode
    {
        public ContentTypeNode(IPowerShellVfs vfs, ContentTypeDefinition definition)
            : base(vfs, definition.Name, definition)
        {
        }
    }
}