// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardVfsNodeExtensions.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Common.Extensions 
{
    using Orchard.Management.PsProvider.Vfs;
    using Proligence.PowerShell.Sites.Items;
    using Proligence.PowerShell.Sites.Nodes;

    /// <summary>
    /// Implements extension methods for the <see cref="OrchardVfsNode"/> class.
    /// </summary>
    public static class OrchardVfsNodeExtensions 
    {
        /// <summary>
        /// Gets the name of the Orchard site which the VFS node belongs to.
        /// </summary>
        /// <param name="node">The orchard VFS node instance.</param>
        /// <returns>
        /// The name of the Orchard site which the VFS node belongs to or <c>null</c> if the node does not belong to
        /// any Orchard site.
        /// </returns>
        public static string GetCurrentSiteName(this OrchardVfsNode node) 
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