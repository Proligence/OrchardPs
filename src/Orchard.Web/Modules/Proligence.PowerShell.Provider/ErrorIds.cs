namespace Proligence.PowerShell.Provider
{
    /// <summary>
    /// Defines well known error identifiers used with PowerShell by the Orchard PS provider.
    /// </summary>
    public static class ErrorIds 
    {
        /// <summary>
        /// Failed to close PowerShell VFS drive.
        /// </summary>
        public const string CloseDriveFailed = "CloseDriveFailed";

        /// <summary>
        /// Failed to create an instance of a <see cref="Proligence.PowerShell.Provider.Vfs.Navigation.IPsNavigationProvider"/>.
        /// </summary>
        public const string CreateNavigationProviderFailed = "CreateNavigationProviderFailed";

        /// <summary>
        /// Failed to get node from PowerShell VFS.
        /// </summary>
        public const string FailedToGetNode = "FailedToGetNode";

        /// <summary>
        /// An exception occurred during handler execution inside PowerShell VFS.
        /// </summary>
        public const string HandlerError = "HandlerError";

        /// <summary>
        /// Item was not found.
        /// </summary>
        public const string ItemNotFound = "ItemNotFound";

        /// <summary>
        /// Failed to enumerate nodes in PowerShell VFS.
        /// </summary>
        public const string NodeEnumerationFailed = "NodeEnumerationFailed";

        /// <summary>
        /// Specified PowerShell VFS drive is <c>null</c>.
        /// </summary>
        public const string NullDrive = "NullDrive";

        /// <summary>
        /// The Orchard PS provider failed to initialize default drives.
        /// </summary>
        public const string DefaultDrivesInitFailed = "DefaultDrivesInitFailed";

        /// <summary>
        /// The specified Orchard root directory is invalid.
        /// </summary>
        public const string InvalidRootDirectory = "InvalidRootDirectory";

        /// <summary>
        /// An Orchard drive was expected for the operation.
        /// </summary>
        public const string OrchardDriveExpected = "OrchardDriveExpected";

        /// <summary>
        /// Failed to initialize Orchard VFS.
        /// </summary>
        public const string OrchardInitFailed = "OrchardInitFailed";

        /// <summary>
        /// Failed to read directory content in the file system.
        /// </summary>
        public const string ReadDirectoryFailed = "ReadDirectoryFailed";

        /// <summary>
        /// Failed to update format files registered in PowerShell.
        /// </summary>
        public const string UpdateFormatDataFailed = "UpdateFormatDataFailed";

        /// <summary>
        /// Failed to find the specified item in the file system.
        /// </summary>
        public const string FailedToFindItem = "FailedToFindItem";
    }
}