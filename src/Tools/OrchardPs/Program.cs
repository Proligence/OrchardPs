namespace OrchardPs 
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Management.Automation.Runspaces;
    using System.Reflection;
    using System.Text;
    using Microsoft.PowerShell;
    using Orchard.Management.PsProvider;

    /// <summary>
    /// Implements the application's entry point.
    /// </summary>
    public static class Program 
    {
        /// <summary>
        /// Implements the application's entry point.
        /// </summary>
        /// <param name="args">The application's command line arguments.</param>
        /// <returns>The application's exit code.</returns>
        public static int Main(string[] args) 
        {
            using (var snapIn = new OrchardPsSnapIn())
            {
                RunspaceConfiguration configuration;
                try
                {
                    configuration = RunspaceConfiguration.Create();

                    foreach (ProviderConfigurationEntry provider in snapIn.Providers)
                    {
                        configuration.Providers.Append(provider);
                    }

                    foreach (CmdletConfigurationEntry cmdlet in snapIn.Cmdlets)
                    {
                        configuration.Cmdlets.Append(cmdlet);
                    }

                    foreach (FormatConfigurationEntry format in snapIn.Formats)
                    {
                        configuration.Formats.Append(format);
                    }

                    configuration.InitializationScripts.Append(
                        new ScriptConfigurationEntry(
                            "NavigateToOrchardDrive",
                            "if (Test-Path Orchard:) { Set-Location Orchard: }"));

                    foreach (KeyValuePair<string, string> alias in snapIn.Aliases)
                    {
                        configuration.InitializationScripts.Append(
                            new ScriptConfigurationEntry(
                                "Alias-" + alias.Key,
                                "New-Alias '" + alias.Key + "' " + alias.Value));
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("Failed to create runspace configuration. " + ex.Message);
                    return -1;
                }

                return ConsoleShell.Start(configuration, GetBanner(), string.Empty, args);
            }
        }

        /// <summary>
        /// Gets the text banner which is displayed to the user when the application is started.
        /// </summary>
        /// <returns>The text banner.</returns>
        private static string GetBanner() 
        {
            var banner = new StringBuilder();
            banner.AppendLine("Proligence Orchard PowerShell");

            var versionAttribute = GetAssemblyAttribute<AssemblyFileVersionAttribute>();
            if (versionAttribute != null) 
            {
                Version version = Assembly.GetEntryAssembly().GetName().Version;
                string versionString = string.Format(
                    CultureInfo.CurrentCulture,
                    "Version {0}.{1} build {2}", 
                    version.Major, 
                    version.Minor, 
                    version.Build);

                banner.Append(versionString);
            }
            
            return banner.ToString();
        }

        /// <summary>
        /// Gets the specified assembly attribute.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute to get.</typeparam>
        /// <returns>The attribute or <c>null</c> if attribute was not found.</returns>
        private static TAttribute GetAssemblyAttribute<TAttribute>()
            where TAttribute : Attribute 
        {
            var assembly = typeof(Program).Assembly;
            var attributes = assembly.GetCustomAttributes(typeof(TAttribute), false);
            if (attributes.Length > 0) 
            {
                return (TAttribute)attributes[0];
            }

            return null;
        }
    }
}