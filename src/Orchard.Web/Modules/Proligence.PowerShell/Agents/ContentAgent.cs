namespace Proligence.PowerShell.Agents 
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.ContentManagement.MetaData;
    using Orchard.ContentManagement.MetaData.Models;

    /// <summary>
    /// Implements the agent which exposes content definitions and data.
    /// </summary>
    public class ContentAgent : IContentAgent
    {
        private readonly IContentDefinitionManager manager;

        public ContentAgent(IContentDefinitionManager manager) 
        {
            this.manager = manager;
        }

        /// <summary>
        /// Gets the definitions of all content fields for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>A sequence of content field definitions.</returns>
        public ContentFieldDefinition[] GetContentFieldDefinitions(string tenant) 
        {
            return manager.ListFieldDefinitions().ToArray();
        }

        /// <summary>
        /// Gets the definitions of all content parts for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>A array of content part definitions.</returns>
        public ContentPartDefinition[] GetContentPartDefinitions(string tenant)
        {
            return manager.ListPartDefinitions().ToArray();
        }

        /// <summary>
        /// Updates the specified content part definition.
        /// </summary>
        /// <param name="definition">The content part definition to update.</param>
        public void UpdateContentPartDefinition(ContentPartDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            if (definition.Settings != null)
            {
                    manager.AlterPartDefinition(
                        definition.Name,
                        part =>
                        {
                            foreach (KeyValuePair<string, string> setting in definition.Settings)
                            {
                                part.WithSetting(setting.Key, setting.Value);
                            }
                        });
            }
        }
    }
}