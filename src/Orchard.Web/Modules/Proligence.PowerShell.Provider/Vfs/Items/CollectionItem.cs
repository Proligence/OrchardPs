using System.Collections;
using System.Linq;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Provider.Vfs.Items {
    /// <summary>
    /// Implements a generic Orchard VFS item object which groups a collection of items in a
    /// <see cref="ContainerNode"/>.
    /// </summary>
    public class CollectionItem {
        /// <summary>
        /// The <see cref="ContainerNode"/> which contains the grouped items.
        /// </summary>
        private readonly ContainerNode _node;

        public CollectionItem(ContainerNode node) {
            _node = node;
        }

        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets the grouped items.
        /// </summary>
        public IEnumerable Items {
            get {
                return _node
                    .GetChildNodes()
                    .Select(n => n.Item)
                    .Where(item => item != null);
            }
        }
    }
}