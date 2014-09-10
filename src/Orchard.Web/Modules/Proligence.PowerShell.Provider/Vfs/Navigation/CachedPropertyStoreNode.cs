namespace Proligence.PowerShell.Provider.Vfs.Navigation
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Implements a VFS node which contains a flat key-value store with non-volatile data.
    /// </summary>
    public abstract class CachedPropertyStoreNode : PropertyStoreNode
    {
        /// <summary>
        /// Contains cached property nodes.
        /// </summary>
        private readonly IDictionary<string, PropertyNode> cache = new Dictionary<string, PropertyNode>();

        protected CachedPropertyStoreNode(IPowerShellVfs vfs, string name)
            : base(vfs, name)
        {
        }

        protected CachedPropertyStoreNode(IPowerShellVfs vfs, string name, IEnumerable<VfsNode> staticNodes)
            : base(vfs, name, staticNodes)
        {
        }

        public override void SetValue(string name, object value)
        {
            this.SetValueInternal(name, value);
            this.cache.Remove(name);
        }

        public override object GetValue(string name)
        {
            PropertyNode propertyNode;
            if (this.cache.TryGetValue(name, out propertyNode))
            {
                return ((DictionaryEntry)propertyNode.Item).Value;
            }

            return this.GetValueInternal(name);
        }

        protected override IEnumerable<VfsNode> GetVirtualNodesInternal()
        {
            return this.GetKeys().Select(
                key =>
                {
                    PropertyNode propertyNode;
                    if (this.cache.TryGetValue(key, out propertyNode))
                    {
                        return propertyNode;
                    }

                    propertyNode = new PropertyNode(this, key, this.GetValueInternal(key));
                    this.cache[key] = propertyNode;

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