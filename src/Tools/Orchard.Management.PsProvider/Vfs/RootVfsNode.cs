// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootVfsNode.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Vfs 
{
    /// <summary>
    /// Represents the root (drive) node in the Orchard VFS.
    /// </summary>
    public class RootVfsNode : ContainerNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootVfsNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance.</param>
        /// <param name="drive">The Orchard drive object.</param>
        public RootVfsNode(IOrchardVfs vfs, OrchardDriveInfo drive) 
            : base(vfs, null) 
        {
            this.Item = drive;
        }
    }
}