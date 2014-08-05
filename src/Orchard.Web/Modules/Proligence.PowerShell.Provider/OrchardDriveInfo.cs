namespace Proligence.PowerShell.Provider
{
    using System.Management.Automation;
    using Autofac;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Represents the state of a single Orchard drive in the Orchard PS provider.
    /// </summary>
    public class OrchardDriveInfo : VfsDriveInfo 
    {
        public OrchardDriveInfo(PSDriveInfo driveInfo, IComponentContext componentContext)
            : base(driveInfo, componentContext)
        {
        }
    }
}