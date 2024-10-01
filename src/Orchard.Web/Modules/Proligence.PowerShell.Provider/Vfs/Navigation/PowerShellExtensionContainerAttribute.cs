using System;

namespace Proligence.PowerShell.Provider.Vfs.Navigation {
    /// <summary>
    /// Marks assemblies which contain extensions for the PowerShell VFS.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public sealed class PowerShellExtensionContainerAttribute : Attribute {
    }
}