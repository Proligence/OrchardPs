namespace Proligence.PowerShell.Agents
{
    using Orchard.ContentManagement.MetaData.Models;

    /// <summary>
    /// Defines the API of the content agent.
    /// </summary>
    public interface IContentAgent : IAgent
    {
        /// <summary>
        /// Gets the definitions of all content fields for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>A array of content field definitions.</returns>
        ContentFieldDefinition[] GetContentFieldDefinitions(string tenant);

        /// <summary>
        /// Gets the definitions of all content parts for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>A array of content part definitions.</returns>
        ContentPartDefinition[] GetContentPartDefinitions(string tenant);

        /// <summary>
        /// Updates the specified content part definition.
        /// </summary>
        /// <param name="definition">The content part definition to update.</param>
        void UpdateContentPartDefinition(ContentPartDefinition definition);
    }
}