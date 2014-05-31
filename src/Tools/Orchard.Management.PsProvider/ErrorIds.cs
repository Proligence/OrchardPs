// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorIds.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    /// <summary>
    /// Defines well known error identifiers used with PowerShell by the Orchard PS provider.
    /// </summary>
    internal static class ErrorIds 
    {
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