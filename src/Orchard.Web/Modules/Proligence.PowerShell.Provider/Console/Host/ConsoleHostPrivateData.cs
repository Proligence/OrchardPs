namespace Proligence.PowerShell.Provider.Console.Host
{
    using Autofac;
    using Proligence.PowerShell.Provider.Internal;

    public class ConsoleHostPrivateData
    {
        /// <summary>
        /// Gets or sets the dependency injection container of the Orchard application.
        /// </summary>
        public IComponentContext ComponentContext { get; set; }

        /// <summary>
        /// Gets or sets the Orchard drive instance.
        /// </summary>
        public OrchardDriveInfo OrchardDrive { get; set; }
    }
}