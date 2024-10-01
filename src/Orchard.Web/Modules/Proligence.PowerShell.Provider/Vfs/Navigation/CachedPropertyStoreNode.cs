using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// Implements a VFS node which contains a flat key-value store with non-volatile data.
    /// </summary>
    public abstract class CachedPropertyStoreNode : PropertyStoreNode {
        /// <summary>
        /// Contains cached property nodes.
        /// </summary>
        private readonly IDictionary<string, PropertyNode> _cache = new Dictionary<string, PropertyNode>();

        protected CachedPropertyStoreNode(IPowerShellVfs vfs, string name)
            : base(vfs, name) {
        }

        protected CachedPropertyStoreNode(IPowerShellVfs vfs, string name, IEnumerable<VfsNode> staticNodes)
            : base(vfs, name, staticNodes) {
        }

        public override void SetValue(string name, object value) {
            SetValueInternal(name, value);
            _cache.Remove(name);
            InvalidateCachedNodes();
        }

        public override object GetValue(string name) {
            PropertyNode propertyNode;
            if (_cache.TryGetValue(name, out propertyNode)) {
                return ((DictionaryEntry) propertyNode.Item).Value;
            }

            return GetValueInternal(name);
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal() {
            return GetKeys().Select(
                key => {
                    PropertyNode propertyNode;
                    if (_cache.TryGetValue(key, out propertyNode)) {
                        return propertyNode;
                    }

                    propertyNode = new PropertyNode(this, key, GetValueInternal(key));
                    _cache[key] = propertyNode;

                    return propertyNode;
                });
        }

        /// <summary>
        /// Sets the value of the specified key. This method should be overridden by subclasses.
        /// </summary>
        /// <param name="name">The name of the key to set.</param>
        /// <param name="value">The value to set.</param>
        protected abstract void SetValueInternal(string name, object value);

        /// <summary>
        /// Gets the value of the specified key. This method should be overridden by subclasses.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <returns>The value of the specified key.</returns>
        protected abstract object GetValueInternal(string name);
    }
}