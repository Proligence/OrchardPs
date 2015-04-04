namespace Proligence.PowerShell.Core.Content.Nodes
{
    using System.Collections.Generic;
    using Proligence.PowerShell.Core.Common.Items;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which contains content-related items of an Orchard tenant.
    /// </summary>
    public class ContentNode : ContainerNode
    {
        public ContentNode(IPowerShellVfs vfs)
            : base(vfs, "Content", CreateStaticNodes(vfs))
        {
            this.Item = new DescriptionItem
            {
                Name = "Content",
                Description = "Contains content definitions and data for the current tenant."
            };
        }

        private static IEnumerable<VfsNode> CreateStaticNodes(IPowerShellVfs vfs)
        {
            yield return new ContentFieldsNode(vfs);
            yield return new ContentPartsNode(vfs);
            yield return new ContentTypesNode(vfs);
        }
    }
}