namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Reflection;
    using System.Web.Hosting;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements the PowerShell snap-in for the Orchard PS provider.
    /// </summary>
    public class OrchardPsSnapIn : CustomPSSnapIn 
    {
        private Collection<ProviderConfigurationEntry> providers;
        private Collection<CmdletConfigurationEntry> cmdlets;
        private Collection<FormatConfigurationEntry> formats;
        private Collection<TypeConfigurationEntry> types;
        private Collection<string> helpFiles;

        public OrchardPsSnapIn()
        {
            this.Aliases = new Dictionary<string, string>();
        }

        public bool Initialized { get; private set; }

        public override string Name 
        {
            get { return "Orchard.Management"; }
        }

        public override string Vendor 
        {
            get { return "Proligence"; }
        }

        public override string Description 
        {
            get 
            {
                return "This Windows PowerShell snap-in contains the Orchard PowerShell provider and Cmdlets " +
                       "defined in Orchard modules.";
            }
        }

        public override Collection<ProviderConfigurationEntry> Providers 
        {
            get { return this.providers; }
        }

        public override Collection<CmdletConfigurationEntry> Cmdlets 
        {
            get { return this.cmdlets; }
        }

        public override Collection<FormatConfigurationEntry> Formats 
        {
            get { return this.formats; }
        }

        public override Collection<TypeConfigurationEntry> Types
        {
            get { return this.types; }
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
            this.types = new Collection<TypeConfigurationEntry>();
            
            DirectoryInfo orchardDirectory = this.GetOrchardDirectory();
            if (orchardDirectory != null)
            {
                this.LoadFormatDataFiles(orchardDirectory.FullName, this.formats);
                this.LoadTypeDataFiles(orchardDirectory.FullName, this.types);
            }

            this.Aliases = new Dictionary<string, string>();

            this.Initialized = true;
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
        /// Discovers the PS type files in Orchard module assemblies.
        /// </summary>
        /// <param name="directory">The Orchard's root directory.</param>
        /// <param name="typesCollection">The collection to which the discovered type files will be added.</param>
        private void LoadTypeDataFiles(string directory, Collection<TypeConfigurationEntry> typesCollection)
        {
            string[] fileNames;
            try
            {
                fileNames = Directory.GetFiles(directory, "*.types.ps1xml", SearchOption.AllDirectories);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Failed to read directory '" + directory + "'. " + ex.Message);
                return;
            }

            foreach (string fileName in fileNames)
            {
                typesCollection.Add(new TypeConfigurationEntry(fileName));
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