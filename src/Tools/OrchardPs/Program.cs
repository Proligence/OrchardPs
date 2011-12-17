using System;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Text;
using Microsoft.PowerShell;
using Orchard.Management.PsProvider;

namespace OrchardPs {
    public static class Program {
        public static int Main(string[] args) {
            RunspaceConfiguration configuration;
            try {
                configuration = RunspaceConfiguration.Create();

                var snapIn = new OrchardPsSnapIn();
                
                foreach (ProviderConfigurationEntry provider in snapIn.Providers) {
                    configuration.Providers.Append(provider);   
                }

                foreach (CmdletConfigurationEntry cmdlet in snapIn.Cmdlets) {
                    configuration.Cmdlets.Append(cmdlet);
                }

                foreach (FormatConfigurationEntry format in snapIn.Formats) {
                    configuration.Formats.Append(format);
                }
            }
            catch (Exception ex) {
                Console.Error.WriteLine("Failed to create runspace configuration. " + ex.Message);
                return -1;
            }

            return ConsoleShell.Start(configuration, GetBanner(), string.Empty, args);
        }

        private static string GetBanner() {
            var banner = new StringBuilder();
            banner.AppendLine("Proligence Orchard PowerShell");

            var versionAttribute = GetAssemblyAttribute<AssemblyFileVersionAttribute>();
            if (versionAttribute != null) {
                banner.Append("Version " + versionAttribute.Version);
            }
            
            return banner.ToString();
        }

        private static TAttribute GetAssemblyAttribute<TAttribute>() where TAttribute : Attribute {
            var assembly = typeof(Program).Assembly;
            var attributes = assembly.GetCustomAttributes(typeof(TAttribute), false);
            if (attributes.Length > 0) {
                return (TAttribute)attributes[0];
            }

            return null;
        }
    }
}