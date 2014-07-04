// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RootVfsNode.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Navigation
{
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Provider;

    /// <summary>
    /// Represents the root (drive) node in the PowerShell VFS.
    /// </summary>
    public class RootVfsNode : ContainerNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootVfsNode"/> class.
        /// </summary>
        /// <param name="vfs">The PowerShell VFS instance.</param>
        /// <param name="drive">The PowerShell VFS drive object.</param>
        public RootVfsNode(IPowerShellVfs vfs, VfsDriveInfo drive) 
            : base(vfs, null) 
        {
            this.Item = drive;
        }
    }
}