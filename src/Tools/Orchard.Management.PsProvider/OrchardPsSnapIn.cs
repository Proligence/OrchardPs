using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;

namespace Orchard.Management.PsProvider {
    public class OrchardPsSnapIn : CustomPSSnapIn {
        private Collection<ProviderConfigurationEntry> _providers;
        private Collection<CmdletConfigurationEntry> _cmdlets;
        private Collection<FormatConfigurationEntry> _formats;
        private Collection<string> _helpFiles;
        private IDictionary<string, string> _aliases;
        private Collection<Assembly> _orchardModuleAssemblies;
        private bool _assembliesLoaded;

        public override string Name {
            get { return "Orchard.Management"; }
        }

        public override string Vendor {
            get { return "Orchard"; }
        }

        public override string Description {
            get { return "This Windows PowerShell snap-in contains the Orchard PowerShell provider and Cmdlets defined in Orchard modules."; }
        }

        public override Collection<ProviderConfigurationEntry> Providers {
            get {
                if (_providers == null) {
                    _providers = new Collection<ProviderConfigurationEntry> {
                        new ProviderConfigurationEntry("Orchard", typeof (OrchardProvider), null)
                    };
                }
                
                return _providers;
            }
        }

        public override Collection<CmdletConfigurationEntry> Cmdlets {
            get {
                LoadOrchardAssemblies();

                if (_cmdlets == null) {
                    _cmdlets = new Collection<CmdletConfigurationEntry>();
                    LoadCmdlets(_cmdlets);
                }

                return _cmdlets;
            }
        }

        public override Collection<FormatConfigurationEntry> Formats {
            get {
                if (_formats == null) {
                    _formats = new Collection<FormatConfigurationEntry>();
                    
                    DirectoryInfo orchardDirectory = GetOrchardDirectory();
                    if (orchardDirectory != null) {
                        LoadFormatDataFiles(orchardDirectory.FullName, _formats);
                    }
                }

                return _formats;
            }
        }

        public IDictionary<string, string> Aliases {
            get {
                if (_aliases == null) {
                    _aliases = new Dictionary<string, string>();
                }

                return _aliases;
            }
        }

        internal static bool VerifyOrchardDirectory(string directory) {
            if (!File.Exists(Path.Combine(directory, "web.config")))
                return false;

            if (!Directory.Exists(Path.Combine(directory, "bin")))
                return false;

            if (!File.Exists(Path.Combine(directory, "bin\\Orchard.Framework.dll")))
                return false;

            return true;
        }

        private void LoadOrchardAssemblies() {
            if (_assembliesLoaded) {
                return;
            }

            string orchardRoot = Directory.GetParent(System.Environment.CurrentDirectory).FullName;
            if (!VerifyOrchardDirectory(orchardRoot)) {
                throw new InvalidOperationException("The current directory does not contain an Orchard installation.");
            }

            _orchardModuleAssemblies = new Collection<Assembly>();
            foreach (string directory in Directory.GetDirectories(Path.Combine(orchardRoot, "Modules"))) {
                DirectoryInfo moduleDirectory = new DirectoryInfo(directory);
                if (moduleDirectory.Exists) {
                    string moduleName = moduleDirectory.Name;
                    string assemblyPath = Path.Combine(moduleDirectory.FullName, Path.Combine("bin", moduleName + ".dll"));
                    if (File.Exists(assemblyPath)) {
                        try {
                            Assembly assembly = Assembly.LoadFrom(assemblyPath);
                            _orchardModuleAssemblies.Add(assembly);
                        }
                        catch (Exception ex) {
                            Trace.WriteLine("Failed to load assembly '" + assemblyPath + "'. " + ex.Message);
                        }
                    }
                }
            }

            _assembliesLoaded = true;
        }

        private void LoadCmdlets(Collection<CmdletConfigurationEntry> cmdlets) {
            Type cmdletType = typeof (Cmdlet);
            foreach (Assembly assembly in _orchardModuleAssemblies) {
                try {
                    foreach (Type type in assembly.GetTypes()) {
                        try {
                            if (cmdletType.IsAssignableFrom(type)) {
                                CmdletAttribute cmdletAttribute =
                                    type.GetCustomAttributes(typeof (CmdletAttribute), false)
                                        .Cast<CmdletAttribute>()
                                        .FirstOrDefault();

                                if (cmdletAttribute == null) {
                                    Trace.WriteLine("Cannot load cmdlet '" + type.FullName + "' because it's class not decorated with CmdletAttribute.");
                                    continue;
                                }

                                string name = cmdletAttribute.VerbName + "-" + cmdletAttribute.NounName;
                                string helpFile = GetHelpFile(name);
                                cmdlets.Add(new CmdletConfigurationEntry(name, type, helpFile));

                                IEnumerable<CmdletAliasAttribute> aliases =
                                    type.GetCustomAttributes(typeof (CmdletAliasAttribute), false)
                                        .Cast<CmdletAliasAttribute>();
                                
                                foreach (CmdletAliasAttribute aliasAttribute in aliases) {
                                    Aliases.Add(aliasAttribute.Alias, name);
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

        private DirectoryInfo GetOrchardDirectory() {
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (directoryName != null) {
                var directoryInfo = new DirectoryInfo(directoryName);
                while ((directoryInfo != null) && !VerifyOrchardDirectory(directoryInfo.FullName)) {
                    directoryInfo = directoryInfo.Parent;
                }

                return directoryInfo;
            }

            return null;
        }

        private void LoadFormatDataFiles(string directory, Collection<FormatConfigurationEntry> formats) {
            string[] fileNames;
            try {
                fileNames = Directory.GetFiles(directory, "*.format.ps1xml", SearchOption.AllDirectories);
            }
            catch (Exception ex) {
                Trace.WriteLine("Failed to read directory '" + directory + "'. " + ex.Message);
                return;
            }

            foreach (string fileName in fileNames) {
                formats.Add(new FormatConfigurationEntry(fileName));
            }
        }

        private string GetHelpFile(string cmdletName) {
            if (_helpFiles == null) {
                _helpFiles = new Collection<string>();

                DirectoryInfo orchardDirectory = GetOrchardDirectory();
                if (orchardDirectory != null) {
                    LoadHelpFiles(orchardDirectory.FullName, _helpFiles);
                }
            }

            return _helpFiles.Where(f => f.Contains(cmdletName + "-help.xml")).FirstOrDefault();
        }

        private void LoadHelpFiles(string directory, Collection<string> helpFiles) {
            string[] fileNames;
            try {
                fileNames = Directory.GetFiles(directory, "*.xml", SearchOption.AllDirectories);
            }
            catch (Exception ex) {
                Trace.WriteLine("Failed to read directory '" + directory + "'. " + ex.Message);
                return;
            }

            foreach (string fileName in fileNames) {
                if (fileName.EndsWith("-help.xml")) {
                    if (!fileName.EndsWith("Orchard.Management.PsProvider.dll-help.xml")) {
                        helpFiles.Add(fileName);
                    }
                }
            }
        }
    }
}