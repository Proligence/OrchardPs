using System.Collections.Generic;
using System.Linq;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// Implements a VFS node which contains a flat key-value store.
    /// </summary>
    public abstract class PropertyStoreNode : ContainerNode {
        protected PropertyStoreNode(IPowerShellVfs vfs, string name)
            : base(vfs, name) {
        }

        protected PropertyStoreNode(IPowerShellVfs vfs, string name, IEnumerable<VfsNode> staticNodes)
            : base(vfs, name, staticNodes) {
        }

        /// <summary>
        /// Gets the keys of the property store.
        /// </summary>
        /// <returns>A sequence of key names.</returns>
        public abstract IEnumerable<string> GetKeys();

        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <param name="name">The name of the key to set.</param>
        /// <param name="value">The value to set.</param>
        public abstract void SetValue(string name, object value);

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <returns>The value of the specified key.</returns>
        public abstract object GetValue(string name);

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() {
            return GetKeys().Select(key => new PropertyNode(this, key, GetValue(key)));
        }
    }
}