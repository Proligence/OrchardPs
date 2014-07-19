namespace Proligence.PowerShell.Agents
{
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Content.Items;

    /// <summary>
    /// Defines the API of the content agent.
    /// </summary>
    [Agent(typeof(ContentAgent))]
    public interface IContentAgent : IAgent
    {
        /// <summary>
        /// Gets the definitions of all content fields for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>A array of content field definitions.</returns>
        OrchardContentFieldDefinition[] GetContentFieldDefinitions(string tenant);

        /// <summary>
        /// Gets the definitions of all content parts for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>A array of content part definitions.</returns>
        OrchardContentPartDefinition[] GetContentPartDefinitions(string tenant);
    }
}