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
        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardDriveInfo"/> class.
        /// </summary>
        public OrchardDriveInfo(PSDriveInfo driveInfo, IComponentContext container)
            : base(driveInfo, container)
        {
        }
    }
}