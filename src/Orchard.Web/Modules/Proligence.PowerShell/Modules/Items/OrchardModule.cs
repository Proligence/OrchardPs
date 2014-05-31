// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardModule.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Modules.Items
{
    using System;

    /// <summary>
    /// Represents an Orchard module.
    /// </summary>
    [Serializable]
    public class OrchardModule
    {
        /// <summary>
        /// Gets or sets the folder name under virtual path base.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the type of the module.
        /// </summary>
        public string ExtensionType { get; set; }

        /// <summary>
        /// Gets or sets the name of the module.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the module.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the module's description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the module's version.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the Orchard version for which the module was designed.
        /// </summary>
        public string OrchardVersion { get; set; }

        /// <summary>
        /// Gets or sets the module's author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the module's website.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets the module's tags.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether anti-forgery support is enable for the module.
        /// </summary>
        public string AntiForgery { get; set; }

        /// <summary>
        /// Gets or sets the zones supported by the module.
        /// </summary>
        public string Zones { get; set; }

        /// <summary>
        /// Gets or sets the name of the module's base theme.
        /// </summary>
        public string BaseTheme { get; set; }

        /// <summary>
        /// Gets or sets the module's session state.
        /// </summary>
        public string SessionState { get; set; }

        /// <summary>
        /// Gets or sets the features exposed by the module.
        /// </summary>
        public OrchardFeature[] Features { get; set; }
    }
}