namespace Proligence.PowerShell.Agents 
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Orchard;
    using Orchard.ContentManagement.MetaData;
    using Orchard.Management.PsProvider.Agents;
    using Proligence.PowerShell.Content.Items;

    /// <summary>
    /// Implements the agent which exposes content definitions and data.
    /// </summary>
    public class ContentAgent : AgentBase, IContentAgent
    {
        /// <summary>
        /// Gets the definitions of all content fields for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>A sequence of content field definitions.</returns>
        public OrchardContentFieldDefinition[] GetContentFieldDefinitions(string tenant)
        {
            using (IWorkContextScope scope = this.CreateWorkContextScope(tenant))
            {
                var manager = scope.Resolve<IContentDefinitionManager>();
                return manager.ListFieldDefinitions()
                    .Select(field => new OrchardContentFieldDefinition { Name = field.Name })
                    .ToArray();
            }
        }

        /// <summary>
        /// Gets the definitions of all content parts for the specified tenant.
        /// </summary>
        /// <param name="tenant">The name of the tenant.</param>
        /// <returns>A array of content part definitions.</returns>
        public OrchardContentPartDefinition[] GetContentPartDefinitions(string tenant)
        {
            using (IWorkContextScope scope = this.CreateWorkContextScope(tenant))
            {
                var manager = scope.Resolve<IContentDefinitionManager>();
                return manager.ListPartDefinitions()
                    .Select(part => 
                        new OrchardContentPartDefinition
                        {
                            Tenant = tenant,
                            Name = part.Name,
                            Fields = part.Fields.Select(
                                field => new OrchardContentFieldDefinition { Name = field.Name }).ToArray(),
                            Settings = new Dictionary<string, string>(part.Settings)
                        })
                    .ToArray();
            }
        }

        /// <summary>
        /// Updates the specified content part definition.
        /// </summary>
        /// <param name="definition">The content part definition to update.</param>
        public void UpdateContentPartDefinition(OrchardContentPartDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException("definition");
            }

            if (definition.Settings != null)
            {
                using (IWorkContextScope scope = this.CreateWorkContextScope(definition.Tenant))
                {
                    scope.Resolve<IContentDefinitionManager>().AlterPartDefinition(
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
}