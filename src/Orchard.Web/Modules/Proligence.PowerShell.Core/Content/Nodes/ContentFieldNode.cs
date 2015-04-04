﻿namespace Proligence.PowerShell.Core.Content.Nodes
{
    using Orchard.ContentManagement.MetaData.Models;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard content field definition.
    /// </summary>
    public class ContentFieldNode : ObjectNode
    {
        public ContentFieldNode(IPowerShellVfs vfs, ContentFieldDefinition definition)
            : base(vfs, definition.Name, definition)
        {
        }
    }
}