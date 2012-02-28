// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Configuration 
{
    using Orchard.Management.PsProvider.Vfs;

    /// <summary>
    /// Implements a VFS node which groups configuration-related nodes for a single Orchard site.
    /// </summary>
    public class ConfigurationNode : ContainerNode 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        public ConfigurationNode(IOrchardVfs vfs)
            : base(vfs, "Configuration") 
        {
            this.Item = "Configuration";
        }
    }
}