﻿using System.Collections;
using System.Linq;
using Orchard.Management.PsProvider.Vfs;

namespace Orchard.PowerShell.Common {
    public class CollectionItem {
        private readonly ContainerNode _node;

        public CollectionItem(ContainerNode node) {
            _node = node;
        }

        public string Name { get; set; }
        public string Description { get; set; }

        public IEnumerable Items {
            get {
                return _node.GetChildNodes()
                    .Select(node => node.Item)
                    .Where(item => item != null);
            }
        }
    }
}