using Proligence.PowerShell.Core.Modules.Items;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Modules.Nodes {
    /// <summary>
    /// Implements a VFS node which represents an Orchard theme.
    /// </summary>
    [SupportedCmdlet("Enable-OrchardTheme")]
    public class ThemeNode : ObjectNode {
        public ThemeNode(IPowerShellVfs vfs, OrchardTheme theme)
            : base(vfs, theme.Name, theme) {
        }

        public override string ToString() {
            return Name;
        }
    }
}