namespace Proligence.PowerShell.Provider.Vfs.Nodes
{
    using Orchard.Environment.Configuration;

    /// <summary>
    /// This class is used to represent the default tenant in the root '$' node.
    /// </summary>
    /// <remarks>
    /// We cannot use the regular <see cref="ShellSettings"/> class, because we need a different class to map
    /// to the root node view in the PowerShell formatting file. Mapping the root view to <see cref="ShellSettings"/>
    /// would also change the view of the 'Tenants' node view.
    /// </remarks>
    public class DefaultShellSettings : ShellSettings
    {
    }
}