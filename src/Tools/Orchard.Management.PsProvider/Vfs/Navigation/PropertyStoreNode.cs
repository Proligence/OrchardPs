namespace Orchard.Management.PsProvider.Vfs.Navigation
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Orchard.Management.PsProvider.Vfs.Core;

    /// <summary>
    /// Implements a VFS node which contains a flat key-value store.
    /// </summary>
    public abstract class PropertyStoreNode : ContainerNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyStoreNode"/> class.
        /// </summary>
        /// <param name="vfs">The VFS instance which the node belongs to.</param>
        /// <param name="name">The name of the node.</param>
        protected PropertyStoreNode(IPowerShellVfs vfs, string name)
            : base(vfs, name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyStoreNode"/> class.
        /// </summary>
        /// <param name="vfs">The VFS instance which the node belongs to.</param>
        /// <param name="name">The name of the node.</param>
        /// <param name="staticNodes">The node's static child items.</param>
        protected PropertyStoreNode(IPowerShellVfs vfs, string name, IEnumerable<VfsNode> staticNodes)
            : base(vfs, name, staticNodes)
        {
        }

        /// <summary>
        /// Gets the node's virtual (dynamic) child nodes.
        /// </summary>
        /// <returns>A sequence of child nodes.</returns>
        public override IEnumerable<VfsNode> GetVirtualNodes()
        {
            return this.GetKeys().Select(key => new PropertyNode(this, key, this.GetValue(key)));
        }

        /// <summary>
        /// Gets the keys of the property store.
        /// </summary>
        /// <returns>A sequence of key names.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
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
    }
}