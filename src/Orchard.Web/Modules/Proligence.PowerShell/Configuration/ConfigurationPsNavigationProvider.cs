// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigurationPsNavigationProvider.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Configuration 
{
    using Orchard.Management.PsProvider.Vfs;

    /// <summary>
    /// Implements the navigation provider which adds the <see cref="ConfigurationNode"/> site node to the Orchard VFS.
    /// </summary>
    public class ConfigurationPsNavigationProvider : PsNavigationProvider 
    {
        /// <summary>
        /// Initializes the navigation provider.
        /// </summary>
        public override void Initialize() 
        {
            NodeType = NodeType.Site;
            Node = new ConfigurationNode(Vfs);
        }
    }
}