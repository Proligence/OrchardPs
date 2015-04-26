namespace Proligence.PowerShell.Core.Content.Nodes
{
    using System.Linq;
    using System.Management.Automation;
    using System.Reflection;
    using Orchard.ContentManagement;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class ContentItemNode : ObjectNode
    {
        public ContentItemNode(IPowerShellVfs vfs, VfsNode parentNode, ContentItem item)
            : base(vfs, item.Id.ToString(), BuildPSObject(item))
        {
            this.Parent = parentNode;
        }

        // ReSharper disable once InconsistentNaming
        private static PSObject BuildPSObject(ContentItem contentItem)
        {
            var psobj = PSObject.AsPSObject(contentItem);

            foreach (ContentPart part in contentItem.Parts)
            {
                foreach (PropertyInfo propertyInfo in part.GetType().GetProperties())
                {
                    object value = propertyInfo.GetValue(part);

                    if (!psobj.Properties.Match(propertyInfo.Name).Any())
                    {
                        psobj.Properties.Add(new PSNoteProperty(propertyInfo.Name, value));
                    }

                    string fullName = part.PartDefinition.Name + "_" + propertyInfo.Name;
                    psobj.Properties.Add(new PSNoteProperty(fullName, value));
                }
            }

            return psobj;
        }
    }
}