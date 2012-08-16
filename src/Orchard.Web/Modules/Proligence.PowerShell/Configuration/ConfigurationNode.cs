// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Configuration 
{
    using Proligence.PowerShell.Vfs;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which groups configuration-related nodes for a single Orchard site.
    /// </summary>
    public class ConfigurationNode : ContainerNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        public ConfigurationNode(IPowerShellVfs vfs)
            : base(vfs, "Configuration") 
        {
            this.Item = "Configuration";
        }
    }
}