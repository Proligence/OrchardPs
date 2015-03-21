namespace Proligence.PowerShell.Provider
{
    using Orchard;

    /// <summary>
    /// Discovers PowerShell content files (help, configuration, etc.) provided by Orchard modules.
    /// </summary>
    public interface IPsFileSearcher : IDependency
    {
        /// <summary>
        /// Gets the path to the help file for the specified cmdlet.
        /// </summary>
        /// <param name="cmdletName">The name of the cmdlet.</param>
        /// <returns>
        /// The path to the cmdlet's help file or <c>null</c> if the cmdlet does not have a help file.
        /// </returns>
        string GetHelpFile(string cmdletName);

        /// <summary>
        /// Discovers the PowerShell format files provided by enabled Orchard modules.
        /// </summary>
        string[] GetFormatDataFiles();

        /// <summary>
        /// Discovers the PowerShell type files provided by enabled Orchard modules.
        /// </summary>
        string[] GetTypeDataFiles();

        /// <summary>
        /// Determines whether the specified file path points to an assembly of an enabled Orchard module.
        /// </summary>
        bool IsEnabledModuleAssembly(string filePath);
    }
}