﻿namespace Proligence.PowerShell.Provider.Vfs.Items 
{
    using System.Collections;
    using System.Linq;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

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
        /// Gets the grouped items.
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