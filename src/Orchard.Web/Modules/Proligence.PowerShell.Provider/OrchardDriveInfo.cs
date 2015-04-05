namespace Proligence.PowerShell.Provider
{
    using System.Management.Automation;
    using Autofac;
    using Proligence.PowerShell.Provider.Vfs.Provider;

    /// <summary>
    /// Represents the state of a PowerShell drive which is handled by the Orchard PS provider.
    /// </summary>
    public class OrchardDriveInfo : VfsDriveInfo 
    {
        public OrchardDriveInfo(PSDriveInfo driveInfo, IComponentContext componentContext)
            : base(driveInfo, componentContext)
        {
        }
    }
}