using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace OrchardPs.Console {
    internal static class Banner {
        public static string GetBanner() {
            var banner = new StringBuilder();
            banner.AppendLine("Proligence Orchard PowerShell");

            var versionAttribute = GetAssemblyAttribute<AssemblyFileVersionAttribute>();
            if (versionAttribute != null) {
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

        private static TAttribute GetAssemblyAttribute<TAttribute>()
            where TAttribute : Attribute {
            var assembly = typeof (Program).Assembly;
            var attributes = assembly.GetCustomAttributes(typeof (TAttribute), false);
            if (attributes.Length > 0) {
                return (TAttribute) attributes[0];
            }

            return null;
        }
    }
}