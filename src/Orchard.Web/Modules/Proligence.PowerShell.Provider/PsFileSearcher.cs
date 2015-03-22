﻿namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Web.Hosting;
    using Orchard;
    using Orchard.Environment.Descriptor.Models;
    using Orchard.Environment.Extensions;

    public class PsFileSearcher : IPsFileSearcher
    {
        private readonly IExtensionManager extensionManager;
        private readonly IWorkContextAccessor wca;
        private string[] enabledModulePaths;
        private Collection<string> helpFiles;

        public PsFileSearcher(IExtensionManager extensionManager, IWorkContextAccessor wca)
        {
            this.extensionManager = extensionManager;
            this.wca = wca;
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
                    this.LoadHelpFiles(modulePath, this.helpFiles);
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
        public bool IsEnabledModuleAssembly(string filePath)
        {
            string fileName = Path.GetFileName(filePath);
            if (fileName != null)
            {
                var moduleName = fileName.Replace(".dll", string.Empty);
                var workContext = this.wca.GetContext();
                if (workContext != null)
                {
                    return this.extensionManager
                        .EnabledFeatures(workContext.Resolve<ShellDescriptor>())
                        .Any(f => f.Id == moduleName);
                }
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

            var workContext = this.wca.GetContext();
            if (workContext != null)
            {
                return this.enabledModulePaths = this.extensionManager
                    .EnabledFeatures(workContext.Resolve<ShellDescriptor>())
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

            return new string[0];
        }

        private void LoadHelpFiles(string directory, Collection<string> helpFilesCollection)
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