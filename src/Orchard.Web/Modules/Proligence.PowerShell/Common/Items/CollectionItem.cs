// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionItem.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Common.Items 
{
    using System.Collections;
    using System.Linq;
    using Proligence.PowerShell.Vfs;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a generic Orchard VFS item object which groups a collection of items in a
    /// <see cref="ContainerNode"/>.
    /// </summary>
    public class CollectionItem 
    {
        /// <summary>
        /// The <see cref="ContainerNode"/> which contains the grouped items.
        /// </summary>
        private readonly ContainerNode node;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionItem"/> class.
        /// </summary>
        /// <param name="node">The <see cref="ContainerNode"/> which contains the grouped items.</param>
        public CollectionItem(ContainerNode node) 
        {
            this.node = node;
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
        /// Gets the groupd items.
        /// </summary>
        public IEnumerable Items 
        {
            get 
            {
                return this.node
                    .GetChildNodes()
                    .Select(n => n.Item)
                    .Where(item => item != null);
            }
        }
    }
}