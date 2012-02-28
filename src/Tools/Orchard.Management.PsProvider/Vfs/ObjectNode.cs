// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectNode.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    /// <summary>
    /// The base class for CFS nodes which encapsulate item objects.
    /// </summary>
    public class ObjectNode : OrchardVfsNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="name">The name of the node.</param>
        /// <param name="obj">The encapsulated item object.</param>
        public ObjectNode(IOrchardVfs vfs, string name, object obj) 
            : base(vfs, name) 
        {
            this.Item = obj;
        }
    }
}