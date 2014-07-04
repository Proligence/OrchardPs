// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorIds.cs" company="Proligence">
//   Copyright (c) Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Vfs.Utils
{
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Defines well known error identifiers used with PowerShell by the PowerShell VFS PS provider.
    /// </summary>
    internal static class ErrorIds 
    {
        /// <summary>
        /// Failed to close PowerShell VFS drive.
        /// </summary>
        public const string CloseDriveFailed = "CloseDriveFailed";

        /// <summary>
        /// Failed to create an instance of a <see cref="IPsNavigationProvider"/>.
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
    }
}