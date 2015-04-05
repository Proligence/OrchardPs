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
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements a PowerShell snap-in which intializes the Orchard PS provider and its extensions from Orchard
    /// modules.
    /// </summary>
    public class OrchardPsSnapIn : CustomPSSnapIn 
    {
        private readonly IPsFileSearcher fileSearcher;
        private Collection<ProviderConfigurationEntry> providers;
        private Collection<CmdletConfigurationEntry> cmdlets;
        private Collection<FormatConfigurationEntry> formats;
        private Collection<TypeConfigurationEntry> types;

        public OrchardPsSnapIn(IPsFileSearcher fileSearcher)
        {
            this.fileSearcher = fileSearcher;
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
        /// Gets a dictionary of command aliases which will be automatically registered in the PowerShell
        /// execution engine.
        /// </summary>
        public IDictionary<string, string> Aliases { get; private set; }

        public void Initialize()
        {
            string helpFilePath = this.fileSearcher.GetHelpFile("Proligence.PowerShell.Provider.dll");

            this.providers = new Collection<ProviderConfigurationEntry>
            {
                new ProviderConfigurationEntry("Orchard", typeof(OrchardProvider), helpFilePath)
            };

            this.cmdlets = new Collection<CmdletConfigurationEntry>();
            this.formats = new Collection<FormatConfigurationEntry>();
            this.types = new Collection<TypeConfigurationEntry>();
            this.Aliases = new Dictionary<string, string>();

            this.LoadCmdlets(this.cmdlets);
            this.LoadFormatDataFiles(this.formats);
            this.LoadTypeDataFiles(this.types);

            this.Initialized = true;
        }

        private void LoadCmdlets(ICollection<CmdletConfigurationEntry> cmdletsCollection)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (!Attribute.IsDefined(assembly, typeof(PowerShellExtensionContainerAttribute)))
                {
                    continue;
                }

                string assemblyName = Path.GetFileName(assembly.Location);
                if (!this.fileSearcher.IsEnabledModuleAssembly(assemblyName))
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
                                this.LoadCmdlet(cmdletsCollection, type);
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

        private void LoadCmdlet(ICollection<CmdletConfigurationEntry> cmdletsCollection, Type type)
        {
            CmdletAttribute cmdletAttribute = type
                .GetCustomAttributes(typeof(CmdletAttribute), false)
                .Cast<CmdletAttribute>()
                .FirstOrDefault();

            if (cmdletAttribute != null)
            {
                string name = cmdletAttribute.VerbName + "-" + cmdletAttribute.NounName;
                string helpFile = this.fileSearcher.GetHelpFile(name);
                cmdletsCollection.Add(new CmdletConfigurationEntry(name, type, helpFile));

                IEnumerable<CmdletAliasAttribute> assemblyAliases = type
                    .GetCustomAttributes(typeof(CmdletAliasAttribute), false)
                    .Cast<CmdletAliasAttribute>();

                foreach (CmdletAliasAttribute aliasAttribute in assemblyAliases)
                {
                    this.Aliases.Add(aliasAttribute.Alias, name);
                }   
            }
            else
            {
                Trace.WriteLine(
                    "Cannot load cmdlet '" + type.FullName + "' because it's class not " +
                    "decorated with CmdletAttribute.");
            }
        }

        private void LoadFormatDataFiles(ICollection<FormatConfigurationEntry> formatsCollection) 
        {
            foreach (string fileName in this.fileSearcher.GetFormatDataFiles())
            {
                formatsCollection.Add(new FormatConfigurationEntry(fileName));
            }
        }

        private void LoadTypeDataFiles(ICollection<TypeConfigurationEntry> typesCollection)
        {
            foreach (string fileName in this.fileSearcher.GetTypeDataFiles())
            {
                typesCollection.Add(new TypeConfigurationEntry(fileName));
            }
        }
    }
}