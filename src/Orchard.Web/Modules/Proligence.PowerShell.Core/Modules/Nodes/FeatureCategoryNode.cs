using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions.Models;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Items;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Modules.Nodes {
    /// <summary>
    /// Implements a VFS node which groups <see cref="FeatureNode"/> nodes of a single feature category.
    /// </summary>
    public class FeatureCategoryNode : ContainerNode {
        private readonly FeatureDescriptor[] _features;

        public FeatureCategoryNode(IPowerShellVfs vfs, string categoryName, FeatureDescriptor[] features)
            : base(vfs, categoryName) {
            _features = features;

            Item = new CollectionItem(this) {
                Name = categoryName,
                Description = "Contains all features of the " + categoryName + " category."
            };
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() {
            return _features.Select(feature => new FeatureNode(Vfs, feature));
        }
    }
}