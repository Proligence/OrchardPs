namespace Proligence.PowerShell.Core.Content.Nodes
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
    using Orchard.ContentManagement;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    [SupportedCmdlet("Update-ContentItem")]
    [SupportedCmdlet("Remove-ContentItem")]
    [SupportedCmdlet("Publish-ContentItem")]
    [SupportedCmdlet("Unpublish-ContentItem")]
    [SupportedCmdlet("Copy-ContentItem")]
    [SupportedCmdlet("Restore-ContentItem")]
    public class ContentItemNode : ObjectNode
    {
        public ContentItemNode(IPowerShellVfs vfs, VfsNode parentNode, ContentItem item)
            : base(vfs, item.Id.ToString(), BuildPSObject(item))
        {
            this.Parent = parentNode;
        }

        // ReSharper disable once InconsistentNaming
        public static PSObject BuildPSObject(ContentItem contentItem)
        {
            var psobj = PSObject.AsPSObject(contentItem);

            foreach (ContentPart part in contentItem.Parts)
            {
                Type partType = part.GetType();
                if (!psobj.Properties.Match(partType.Name).Any())
                {
                    psobj.Properties.Add(new PSNoteProperty(partType.Name, part));
                }

                foreach (PropertyInfo propertyInfo in partType.GetProperties())
                {
                    object value = propertyInfo.GetValue(part);

                    if (!psobj.Properties.Match(propertyInfo.Name).Any())
                    {
                        psobj.Properties.Add(new PSNoteProperty(propertyInfo.Name, value));
                    }
                }
            }

            return psobj;
        }
    }
}