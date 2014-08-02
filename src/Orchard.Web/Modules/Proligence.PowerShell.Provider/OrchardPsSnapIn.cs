using System.Web.Hosting;

namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Reflection;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements the PowerShell snap-in for the Orchard PS provider.
    /// </summary>
    public class OrchardPsSnapIn : CustomPSSnapIn 
    {
        private Collection<ProviderConfigurationEntry> providers;
        private Collection<CmdletConfigurationEntry> cmdlets;
        private Collection<FormatConfigurationEntry> formats;
        private Collection<string> helpFiles;

        public OrchardPsSnapIn()
        {
            this.Aliases = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the name of the snap-in.
        /// </summary>
        public override string Name 
        {
            get { return "Orchard.Management"; }
        }

        /// <summary>
        /// Gets the name of the snap-in's vendor.
        /// </summary>
        public override string Vendor 
        {
            get { return "Proligence"; }
        }

        /// <summary>
        /// Gets the snap-in's description.
        /// </summary>
        public override string Description 
        {
            get 
            {
                return "This Windows PowerShell snap-in contains the Orchard PowerShell provider and Cmdlets " +
                       "defined in Orchard modules.";
            }
        }

        /// <summary>
        /// Gets a collection of PS providers included in the snap-in.
        /// </summary>
        public override Collection<ProviderConfigurationEntry> Providers 
        {
            get { return this.providers; }
        }

        /// <summary>
        /// Gets a collection of PS cmdlets included in the snap-in.
        /// </summary>
        public override Collection<CmdletConfigurationEntry> Cmdlets 
        {
            get { return this.cmdlets; }
        }

        /// <summary>
        /// Gets a collection of PS object formats included in the snap-in.
        /// </summary>
        public override Collection<FormatConfigurationEntry> Formats 
        {
            get { return this.formats; }
        }

        /// <summary>
        /// Gets a dictionary of command aliases which will be automatically created.
        /// </summary>
        public IDictionary<string, string> Aliases { get; private set; }

        /// <summary>
        /// Initializes the snap-in and fills its collections.
        /// </summary>
        public void Initialize()
        {
            this.LoadHelpFiles();
            
            string helpFilePath = this.GetHelpFile("Proligence.PowerShell.Provider.dll");

            this.providers = new Collection<ProviderConfigurationEntry>
            {
                new ProviderConfigurationEntry("Orchard", typeof(OrchardProvider), helpFilePath)
            };

            this.cmdlets = new Collection<CmdletConfigurationEntry>();
            this.LoadCmdlets(this.cmdlets);

            this.formats = new Collection<FormatConfigurationEntry>();
            DirectoryInfo orchardDirectory = this.GetOrchardDirectory();
            if (orchardDirectory != null)
            {
                this.LoadFormatDataFiles(orchardDirectory.FullName, this.formats);
            }

            this.Aliases = new Dictionary<string, string>();
        }

        /// <summary>
        /// Discovers the cmdlets defined in Orchard module assemblies.
        /// </summary>
        /// <param name="cmdletsCollection">The collection to which the discovered assemblies will be added.</param>
        private void LoadCmdlets(ICollection<CmdletConfigurationEntry> cmdletsCollection)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!Attribute.IsDefined(assembly, typeof(PowerShellExtensionContainerAttribute)))
                {
                    continue;
                }

                try
                {
                    foreach (Type type in assembly.GetTypes())
                    {
                        try
                        {
                            if (typeof(Cmdlet).IsAssignableFrom(type))
                            {
                                CmdletAttribute cmdletAttribute =
                                    type.GetCustomAttributes(typeof(CmdletAttribute), false)
                                        .Cast<CmdletAttribute>()
                                        .FirstOrDefault();

                                if (cmdletAttribute == null)
                                {
                                    Trace.WriteLine(
                                        "Cannot load cmdlet '" + type.FullName + "' because it's class not " + 
                                        "decorated with CmdletAttribute.");
                                    continue;
                                }

                                string name = cmdletAttribute.VerbName + "-" + cmdletAttribute.NounName;
                                string helpFile = this.GetHelpFile(name);
                                cmdletsCollection.Add(new CmdletConfigurationEntry(name, type, helpFile));

                                IEnumerable<CmdletAliasAttribute> assemblyAliases =
                                    type.GetCustomAttributes(typeof(CmdletAliasAttribute), false)
                                        .Cast<CmdletAliasAttribute>();
                                
                                foreach (CmdletAliasAttribute aliasAttribute in assemblyAliases) 
                                {
                                    this.Aliases.Add(aliasAttribute.Alias, name);
                                }
                            }
                        }
                        catch (Exception ex) 
                        {
                            Trace.WriteLine("Failed to process type '" + type.FullName + "'. " + ex.Message);
                        }
                    }
                }
                catch (Exception ex) 
                {
                    Trace.WriteLine("Failed to get types for assembly '" + assembly.FullName + "'. " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Gets the Orchard directory based on the location of the application's entry assembly.
        /// </summary>
        /// <returns>
        /// The <see cref="DirectoryInfo"/> object of the Orchard's root directory or <c>null</c> if Orchard directory
        /// was not found.
        /// </returns>
        private DirectoryInfo GetOrchardDirectory() 
        {
            return new DirectoryInfo(HostingEnvironment.ApplicationPhysicalPath);
        }

        /// <summary>
        /// Discovers the PS format files in Orchard module assemblies.
        /// </summary>
        /// <param name="directory">The Orchard's root directory.</param>
        /// <param name="formatsCollection">The collection to which the discovered format files will be added.</param>
        private void LoadFormatDataFiles(string directory, Collection<FormatConfigurationEntry> formatsCollection) 
        {
            string[] fileNames;
            try 
            {
                fileNames = Directory.GetFiles(directory, "*.format.ps1xml", SearchOption.AllDirectories);
            }
            catch (Exception ex) 
            {
                Trace.WriteLine("Failed to read directory '" + directory + "'. " + ex.Message);
                return;
            }

            foreach (string fileName in fileNames) 
            {
                formatsCollection.Add(new FormatConfigurationEntry(fileName));
            }
        }

        /// <summary>
        /// Gets the path to the help file for the specified cmdlet.
        /// </summary>
        /// <param name="cmdletName">The name of the cmdlet.</param>
        /// <returns>
        /// The path to the cmdlet's help file or <c>null</c> if the cmdlet does not have a help file.
        /// </returns>
        private string GetHelpFile(string cmdletName) 
        {
            if (this.helpFiles == null) 
            {
                this.LoadHelpFiles();
            }

            return this.helpFiles.FirstOrDefault(f => f.Contains(cmdletName + "-help.xml"));
        }

        /// <summary>
        /// Discovers the PS cmdlet help files in Orchard module assemblies.
        /// </summary>
        private void LoadHelpFiles()
        {
            this.helpFiles = new Collection<string>();

            DirectoryInfo orchardDirectory = this.GetOrchardDirectory();
            if (orchardDirectory != null)
            {
                this.LoadHelpFiles(orchardDirectory.FullName, this.helpFiles);
            }
        }

        /// <summary>
        /// Discovers the PS cmdlet help files in Orchard module assemblies.
        /// </summary>
        /// <param name="directory">The Orchard's root directory.</param>
        /// <param name="helpFilesCollection">The collection to which the discovered help files will be added.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design")]
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
    }
}