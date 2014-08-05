namespace Proligence.PowerShell.Modules.Nodes
{
    using Proligence.PowerShell.Modules.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents an Orchard theme.
    /// </summary>
    [SupportedCmdlet("Enable-OrchardTheme")]
    public class ThemeNode : ObjectNode
    {
        public ThemeNode(IPowerShellVfs vfs, OrchardTheme theme)
            : base(vfs, theme.Name, theme)
        {
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}