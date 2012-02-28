// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorIds.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using Orchard.Management.PsProvider.Vfs;

    /// <summary>
    /// Defines well known error identifiers used with PowerShell by the Orchard PS provider.
    /// </summary>
    internal static class ErrorIds 
    {
        /// <summary>
        /// Failed to close Orchard drive.
        /// </summary>
        public const string CloseDriveFailed = "CloseDriveFailed";

        /// <summary>
        /// Failed to create an instance of a <see cref="IPsNavigationProvider"/>.
        /// </summary>
        public const string CreateNavigationProviderFailed = "CreateNavigationProviderFailed";

        /// <summary>
        /// The Orchard PS provider failed to initialize default drives.
        /// </summary>
        public const string DefaultDrivesInitFailed = "DefaultDrivesInitFailed";

        /// <summary>
        /// Failed to get node from orchard VFS.
        /// </summary>
        public const string FailedToGetNode = "FailedToGetNode";

        /// <summary>
        /// An exception occurred during handler execution inside Orchard VFS.
        /// </summary>
        public const string HandlerError = "HandlerError";

        /// <summary>
        /// The specified Orchard root directory is invalid.
        /// </summary>
        public const string InvalidRootDirectory = "InvalidRootDirectory";

        /// <summary>
        /// Item was not found.
        /// </summary>
        public const string ItemNotFound = "ItemNotFound";

        /// <summary>
        /// Failed to enumerate nodes in Orchard VFS.
        /// </summary>
        public const string NodeEnumerationFailed = "NodeEnumerationFailed";

        /// <summary>
        /// Specified Orchard drive is <c>null</c>.
        /// </summary>
        public const string NullDrive = "NullDrive";

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
    }
}