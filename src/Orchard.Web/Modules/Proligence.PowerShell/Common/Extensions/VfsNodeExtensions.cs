// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VfsNodeExtensions.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Common.Extensions 
{
    using Proligence.PowerShell.Sites.Items;
    using Proligence.PowerShell.Sites.Nodes;
    using Proligence.PowerShell.Vfs;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements extension methods for the <see cref="VfsNode"/> class.
    /// </summary>
    public static class VfsNodeExtensions 
    {
        /// <summary>
        /// Gets the name of the Orchard site which the VFS node belongs to.
        /// </summary>
        /// <param name="node">The orchard VFS node instance.</param>
        /// <returns>
        /// The name of the Orchard site which the VFS node belongs to or <c>null</c> if the node does not belong to
        /// any Orchard site.
        /// </returns>
        public static string GetCurrentSiteName(this VfsNode node) 
        {
            while (node != null) 
            {
                var siteNode = node as SiteNode;
                if (siteNode != null) 
                {
                    return ((OrchardSite)siteNode.Item).Name;
                }

                node = node.Parent;
            }

            return null;
        }
    }
}