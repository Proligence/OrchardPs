// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyNode.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Navigation
{
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Implements a VFS node which maps a name to a single value.
    /// </summary>
    public class PropertyNode : ObjectNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyNode"/> class.
        /// </summary>
        /// <param name="parent">The <see cref="PropertyStoreNode"/> node which is the parent of this node.</param>
        /// <param name="name">The name of the node.</param>
        /// <param name="value">The value stored in the node.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        public PropertyNode(PropertyStoreNode parent, string name, object value)
            : base(parent.Vfs, name, new DictionaryEntry(name, value))
        {
            this.SetItemHandler = (node, v) => parent.SetValue(node.Name, v);
        }
    }
}