// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardCmdletExtensions.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Common.Extensions 
{
    using System;
    using Orchard.Management.PsProvider;
    using Orchard.Management.PsProvider.Vfs;
    using Proligence.PowerShell.Sites.Items;
    using Proligence.PowerShell.Sites.Nodes;

    /// <summary>
    /// Implements extension methods for the <see cref="OrchardCmdlet"/> class.
    /// </summary>
    public static class OrchardCmdletExtensions 
    {
        /// <summary>
        /// Gets the Orchard site from which the cmdlet was invoked.
        /// </summary>
        /// <param name="cmdlet">The cmdlet instance.</param>
        /// <returns>
        /// The <see cref="OrchardSite"/> object which represents the current site or <c>null</c> if the cmdlet was
        /// invoked from a path which is not under any Orchard site.
        /// </returns>
        public static OrchardSite GetCurrentSite(this OrchardCmdlet cmdlet) 
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            OrchardVfsNode currentNode = cmdlet.CurrentNode;
            while (currentNode != null) 
            {
                var siteNode = currentNode as SiteNode;
                if (siteNode != null) 
                {
                    return siteNode.Item as OrchardSite;
                }
                
                currentNode = currentNode.Parent;
            }

            return null;
        }
    }
}