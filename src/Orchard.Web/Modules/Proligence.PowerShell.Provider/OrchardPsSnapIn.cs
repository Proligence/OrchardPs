using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Proligence.PowerShell.Provider.Internal;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Provider {
    /// <summary>
    /// Implements a PowerShell snap-in which initializes the Orchard PS provider and its extensions from Orchard
    /// modules.
    /// </summary>
    public class OrchardPsSnapIn : CustomPSSnapIn {
        private readonly IPsFileSearcher _fileSearcher;
        private readonly IExtensionManager _extensionManager;
        private readonly ShellDescriptor _shellDescriptor;
        private Collection<ProviderConfigurationEntry> _providers;
        private Collection<CmdletConfigurationEntry> _cmdlets;
        private Collection<FormatConfigurationEntry> _formats;
        private Collection<TypeConfigurationEntry> _types;

        public OrchardPsSnapIn(IPsFileSearcher fileSearcher, IExtensionManager extensionManager, ShellDescriptor shellDescriptor) {
            _fileSearcher = fileSearcher;
            _extensionManager = extensionManager;
            _shellDescriptor = shellDescriptor;
            Aliases = new Dictionary<string, string>();
        }

        public bool Initialized { get; private set; }

        public override string Name {
            get { return "Orchard.Management"; }
        }

        public override string Vendor {
            get { return "Proligence"; }
        }

        public override string Description {
            get {
                return "This Windows PowerShell snap-in contains the Orchard PowerShell provider and Cmdlets " +
                       "defined in Orchard modules.";
            }
        }

        public override Collection<ProviderConfigurationEntry> Providers {
            get { return _providers; }
        }

        public override Collection<CmdletConfigurationEntry> Cmdlets {
            get { return _cmdlets; }
        }

        public override Collection<FormatConfigurationEntry> Formats {
            get { return _formats; }
        }

        public override Collection<TypeConfigurationEntry> Types {
            get { return _types; }
        }

        /// <summary>
        /// Gets a dictionary of command aliases which will be automatically registered in the PowerShell
        /// execution engine.
        /// </summary>
        public IDictionary<string, string> Aliases { get; private set; }

        public void Initialize() {
            string helpFilePath = _fileSearcher.GetHelpFile("Proligence.PowerShell.Provider.dll");

            _providers = new Collection<ProviderConfigurationEntry> {
                new ProviderConfigurationEntry("Orchard", typeof (OrchardProvider), helpFilePath)
            };

            _cmdlets = new Collection<CmdletConfigurationEntry>();
            _formats = new Collection<FormatConfigurationEntry>();
            _types = new Collection<TypeConfigurationEntry>();
            Aliases = new Dictionary<string, string>();

            LoadCmdlets(_cmdlets);
            LoadFormatDataFiles(_formats);
            LoadTypeDataFiles(_types);

            Initialized = true;
        }

        private void LoadCmdlets(ICollection<CmdletConfigurationEntry> cmdletsCollection) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies) {
                if (!Attribute.IsDefined(assembly, typeof (PowerShellExtensionContainerAttribute))) {
                    continue;
                }

                string assemblyName = Path.GetFileName(assembly.Location);
                if (!_fileSearcher.IsEnabledModuleAssembly(assemblyName)) {
                    continue;
                }

                try {
                    foreach (Type type in assembly.GetTypes()) {
                        try {
                            if (typeof (Cmdlet).IsAssignableFrom(type)) {
                                var featureAttr = (OrchardFeatureAttribute) Attribute.GetCustomAttribute(
                                    type,
                                    typeof (OrchardFeatureAttribute));

                                if ((featureAttr == null) || IsFeatureEnabled(featureAttr.FeatureName)) {
                                    LoadCmdlet(cmdletsCollection, type);
                                }
                            }
                        }
                        catch (Exception ex) {
                            Trace.WriteLine("Failed to process type '" + type.FullName + "'. " + ex.Message);
                        }
                    }
                }
                catch (Exception ex) {
                    Trace.WriteLine("Failed to get types for assembly '" + assembly.FullName + "'. " + ex.Message);
                }
            }
        }

        private void LoadCmdlet(ICollection<CmdletConfigurationEntry> cmdletsCollection, Type type) {
            CmdletAttribute cmdletAttribute = type
                .GetCustomAttributes(typeof (CmdletAttribute), false)
                .Cast<CmdletAttribute>()
                .FirstOrDefault();

            if (cmdletAttribute != null) {
                string name = cmdletAttribute.VerbName + "-" + cmdletAttribute.NounName;
                string helpFile = _fileSearcher.GetHelpFile(name);
                cmdletsCollection.Add(new CmdletConfigurationEntry(name, type, helpFile));

                IEnumerable<CmdletAliasAttribute> assemblyAliases = type
                    .GetCustomAttributes(typeof (CmdletAliasAttribute), false)
                    .Cast<CmdletAliasAttribute>();

                foreach (CmdletAliasAttribute aliasAttribute in assemblyAliases) {
                    Aliases.Add(aliasAttribute.Alias, name);
                }
            }
            else {
                Trace.WriteLine(
                    "Cannot load cmdlet '" + type.FullName + "' because it's class not " +
                    "decorated with CmdletAttribute.");
            }
        }

        private void LoadFormatDataFiles(ICollection<FormatConfigurationEntry> formatsCollection) {
            foreach (string fileName in _fileSearcher.GetFormatDataFiles()) {
                formatsCollection.Add(new FormatConfigurationEntry(fileName));
            }
        }

        private void LoadTypeDataFiles(ICollection<TypeConfigurationEntry> typesCollection) {
            foreach (string fileName in _fileSearcher.GetTypeDataFiles()) {
                typesCollection.Add(new TypeConfigurationEntry(fileName));
            }
        }

        private bool IsFeatureEnabled(string featureId) {
            return _extensionManager
                .EnabledFeatures(_shellDescriptor)
                .Any(f => f.Id == featureId);
        }
    }
}