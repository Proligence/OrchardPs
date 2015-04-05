namespace Proligence.PowerShell.Provider.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using Orchard.Environment.Descriptor.Models;
    using Orchard.Environment.Extensions;

    public class PsFileSearcher : IPsFileSearcher
    {
        private readonly IExtensionManager extensionManager;
        private readonly ShellDescriptor shellDescriptor;
        private string[] enabledModulePaths;
        private Collection<string> helpFiles;

        public PsFileSearcher(IExtensionManager extensionManager, ShellDescriptor shellDescriptor)
        {
            this.extensionManager = extensionManager;
            this.shellDescriptor = shellDescriptor;
        }

        /// <summary>
        /// Gets the path to the help file for the specified cmdlet.
        /// </summary>
        /// <param name="cmdletName">The name of the cmdlet.</param>
        /// <returns>
        /// The path to the cmdlet's help file or <c>null</c> if the cmdlet does not have a help file.
        /// </returns>
        public string GetHelpFile(string cmdletName)
        {
            if (this.helpFiles == null)
            {
                this.helpFiles = new Collection<string>();
                
                foreach (string modulePath in this.GetEnabledModulePaths())
                {
                    LoadHelpFiles(modulePath, this.helpFiles);
                }
            }

            return this.helpFiles.FirstOrDefault(f => f.Contains(cmdletName + "-help.xml"));
        }

        /// <summary>
        /// Discovers the PowerShell format files provided by enabled Orchard modules.
        /// </summary>
        public string[] GetFormatDataFiles()
        {
            return this.SearchFiles("*.format.ps1xml");
        }

        /// <summary>
        /// Discovers the PowerShell type files provided by enabled Orchard modules.
        /// </summary>
        public string[] GetTypeDataFiles()
        {
            return this.SearchFiles("*.types.ps1xml");
        }

        /// <summary>
        /// Determines whether the specified file path points to an assembly of an enabled Orchard module.
        /// </summary>
        public bool IsEnabledModuleAssembly(string assemblyName)
        {
            if (assemblyName != null)
            {
                var moduleName = assemblyName.Replace(".dll", string.Empty);
                
                return this.extensionManager
                    .EnabledFeatures(this.shellDescriptor)
                    .Any(f => f.Id == moduleName);
            }

            return false;
        }

        /// <summary>
        /// Gets the root paths of all enabled Orchard modules.
        /// </summary>
        private string[] GetEnabledModulePaths()
        {
            if (this.enabledModulePaths != null)
            {
                return this.enabledModulePaths;
            }
            
            return this.enabledModulePaths = this.extensionManager
                .EnabledFeatures(this.shellDescriptor)
                .Select(f =>
                {
                    string location = f.Extension.Location;
                    string fullLocation = location.Replace("~/", HostingEnvironment.ApplicationPhysicalPath);
                    if (location == "~/Modules")
                    {
                        return Path.Combine(fullLocation, f.Extension.Id);
                    }

                    return fullLocation;
                })
                .Distinct()
                .Where(Directory.Exists)
                .ToArray();
        }

        private static void LoadHelpFiles(string directory, ICollection<string> helpFilesCollection)
        {
            string[] fileNames;
            try
            {
                fileNames = Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to read directory '" + directory + "'. " + ex.Message);
                return;
            }

            foreach (string fileName in fileNames)
            {
                if (fileName.EndsWith("-help.xml", StringComparison.OrdinalIgnoreCase))
                {
                    helpFilesCollection.Add(fileName);
                }
            }
        }

        private string[] SearchFiles(string searchPattern)
        {
            var result = new List<string>();

            foreach (string modulePath in this.GetEnabledModulePaths())
            {
                try
                {
                    result.AddRange(Directory.GetFiles(modulePath, searchPattern, SearchOption.AllDirectories));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Failed to read directory '" + modulePath + "'. " + ex.Message);
                }
            }

            return result.ToArray();
        }
    }
}