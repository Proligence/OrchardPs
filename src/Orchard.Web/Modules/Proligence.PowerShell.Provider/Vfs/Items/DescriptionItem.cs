namespace Proligence.PowerShell.Provider.Vfs.Items {
    /// <summary>
    /// Implements a generic Orchard VFS item object which contains a description.
    /// </summary>
    public class DescriptionItem {
        /// <summary>
        /// Gets or sets the name of the item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the item.
        /// </summary>
        public string Description { get; set; }
    }
}