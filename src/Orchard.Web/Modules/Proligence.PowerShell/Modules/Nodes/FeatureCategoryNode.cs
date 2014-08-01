// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FeatureCategoryNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.Nodes 
{
    using System.Collections.Generic;
    using System.Linq;

    using Orchard.Environment.Extensions.Models;

    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups <see cref="FeatureNode"/> nodes of a single feature category.
    /// </summary>
    public class FeatureCategoryNode : ContainerNode
    {
        /// <summary>
        /// The encapsulated Orchard features.
        /// </summary>
        private readonly FeatureDescriptor[] features;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureCategoryNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="categoryName">The name of the feature category.</param>
        /// <param name="features">The objects which represent the Orchard features to encapsulate.</param>
        public FeatureCategoryNode(IPowerShellVfs vfs, string categoryName, FeatureDescriptor[] features)
            : base(vfs, categoryName)
        {
            this.features = features;

            this.Item = new CollectionItem(this) 
            {
                Name = categoryName,
                Description = "Contains all features of the " + categoryName + " category."
            };
        }

        /// <summary>
        /// Gets the node's virtual (dynamic) child nodes.
        /// </summary>
        /// <returns>A sequence of child nodes.</returns>
        public override IEnumerable<VfsNode> GetVirtualNodes()
        {
            return features.Select(feature => new FeatureNode(this.Vfs, feature));
        }
    }
}