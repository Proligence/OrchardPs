namespace Proligence.PowerShell.Modules.Nodes
{
    using System.Diagnostics.CodeAnalysis;
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs.Core;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard theme.
    /// </summary>
    [SupportedCmdlet("Enable-OrchardTheme")]
    public class ThemeNode : ObjectNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeNode"/> class.
        /// </summary>
        /// <param name="vfs">The Orchard VFS instance which the node belongs to.</param>
        /// <param name="theme">The object of the Orchard theme represented by the node.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods")]
        public ThemeNode(IPowerShellVfs vfs, OrchardTheme theme)
            : base(vfs, theme.Name, theme)
        {
        }

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return this.Name;
        }
    }
}