namespace Orchard.Management.PsProvider.Vfs.Navigation
{
    using System;

    /// <summary>
    /// Marks assemblies which contain extensions for the PowerShell VFS.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
    public sealed class PowerShellExtensionContainerAttribute : Attribute
    {
    }
}