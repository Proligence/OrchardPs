namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Management.Automation;
    using System.Management.Automation.Runspaces;
    using System.Reflection;
    using Orchard;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    /// <summary>
    /// Implements the PowerShell snap-in for the Orchard PS provider.
    /// </summary>
    public class OrchardPsSnapIn : CustomPSSnapIn 
    {
        private readonly IWorkContextAccessor wca;
        private Collection<ProviderConfigurationEntry> providers;
        private Collection<CmdletConfigurationEntry> cmdlets;
        private Collection<FormatConfigurationEntry> formats;
        private Collection<TypeConfigurationEntry> types;

        public OrchardPsSnapIn(IWorkContextAccessor wca)
        {
            this.wca = wca;
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
            var fileSearcher = this.wca.GetContext().Resolve<IPsFileSearcher>();
            
            string helpFilePath = fileSearcher.GetHelpFile("Proligence.PowerShell.Provider.dll");

            this.providers = new Collection<ProviderConfigurationEntry>
            {
                new ProviderConfigurationEntry("Orchard", typeof(OrchardProvider), helpFilePath)
            };

            this.cmdlets = new Collection<CmdletConfigurationEntry>();
            this.formats = new Collection<FormatConfigurationEntry>();
            this.types = new Collection<TypeConfigurationEntry>();
            this.Aliases = new Dictionary<string, string>();

            this.LoadCmdlets(this.cmdlets, fileSearcher);
            this.LoadFormatDataFiles(this.formats, fileSearcher);
            this.LoadTypeDataFiles(this.types, fileSearcher);

            this.Initialized = true;
        }

        private void LoadCmdlets(ICollection<CmdletConfigurationEntry> cmdletsCollection, IPsFileSearcher fileSearcher)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                if (!Attribute.IsDefined(assembly, typeof(PowerShellExtensionContainerAttribute)))
                {
                    continue;
                }

                if (!fileSearcher.IsEnabledModuleAssembly(assembly.Location))
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
                                this.LoadCmdlet(cmdletsCollection, type, fileSearcher);
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

        private void LoadCmdlet(ICollection<CmdletConfigurationEntry> cmdletsCollection, Type type, IPsFileSearcher fileSearcher)
        {
            CmdletAttribute cmdletAttribute = type
                .GetCustomAttributes(typeof(CmdletAttribute), false)
                .Cast<CmdletAttribute>()
                .FirstOrDefault();

            if (cmdletAttribute != null)
            {
                string name = cmdletAttribute.VerbName + "-" + cmdletAttribute.NounName;
                string helpFile = fileSearcher.GetHelpFile(name);
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

        private void LoadFormatDataFiles(ICollection<FormatConfigurationEntry> formatsCollection, IPsFileSearcher fileSearcher) 
        {
            foreach (string fileName in fileSearcher.GetFormatDataFiles())
            {
                formatsCollection.Add(new FormatConfigurationEntry(fileName));
            }
        }

        private void LoadTypeDataFiles(ICollection<TypeConfigurationEntry> typesCollection, IPsFileSearcher fileSearcher)
        {
            foreach (string fileName in fileSearcher.GetTypeDataFiles())
            {
                typesCollection.Add(new TypeConfigurationEntry(fileName));
            }
        }
    }
}