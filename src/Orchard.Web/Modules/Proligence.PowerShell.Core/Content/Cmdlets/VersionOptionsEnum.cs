using Orchard.ContentManagement;

namespace Proligence.PowerShell.Core.Content.Cmdlets {
    /// <summary>
    /// This enum was introduced so that different version options can be specified as cmdlet parameters. We need an
    /// enum to support that and the Orchard <see cref="VersionOptions"/> uses properties and constructors.
    /// </summary>
    public enum VersionOptionsEnum {
        Latest,
        Published,
        Draft,
        DraftRequired,
        AllVersions
    }

    public static class VersionOptionsEnumExtensions {
        public static VersionOptions ToVersionOptions(this VersionOptionsEnum versionOptions) {
            switch (versionOptions) {
                case VersionOptionsEnum.Latest:
                    return VersionOptions.Latest;
                case VersionOptionsEnum.Published:
                    return VersionOptions.Published;
                case VersionOptionsEnum.Draft:
                    return VersionOptions.Draft;
                case VersionOptionsEnum.DraftRequired:
                    return VersionOptions.DraftRequired;
                case VersionOptionsEnum.AllVersions:
                    return VersionOptions.AllVersions;
                default:
                    return null;
            }
        }
    }
}