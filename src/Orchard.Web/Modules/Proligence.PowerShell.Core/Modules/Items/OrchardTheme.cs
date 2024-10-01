﻿using System;
using Orchard.Environment.Extensions.Models;

namespace Proligence.PowerShell.Core.Modules.Items {
    [Serializable]
    public class OrchardTheme {
        /// <summary>
        /// Gets or sets the Orchard module which provides the theme.
        /// </summary>
        public ExtensionDescriptor Module { get; set; }

        /// <summary>
        /// Gets the identifier of the theme.
        /// </summary>
        public string Id {
            get { return Module.Id; }
        }

        /// <summary>
        /// Gets the theme's name.
        /// </summary>
        public string Name {
            get { return Module.Name; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the theme is currently activated.
        /// </summary>
        public bool Activated { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the theme needs a data update / migration.
        /// </summary>
        public bool NeedsUpdate { get; set; }

        /// <summary>
        /// Gets or sets the name of the Orchard tenant which the feature belongs to.
        /// </summary>
        public string TenantName { get; set; }

        public override string ToString() {
            return Name;
        }
    }
}