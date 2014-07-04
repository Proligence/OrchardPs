// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TenantConfigurationNode.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Tenants.Nodes
{
    using System.Collections.Generic;
    using System.Linq;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Vfs.Core;
    using Proligence.PowerShell.Vfs.Navigation;

    /// <summary>
    /// Implements a VFS node which represents the configuration of an Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Set-Item")]
    public class TenantConfigurationNode : CachedPropertyStoreNode
    {
        /// <summary>
        /// The tenant agent.
        /// </summary>
        private readonly ITenantAgent agent;

        /// <summary>
        /// Contains cached tenant setting names.
        /// </summary>
        private string[] keys;

        /// <summary>Initializes a new instance of the <see cref="TenantConfigurationNode"/> class.</summary>
        /// <param name="vfs">The VFS instance which the node belongs to.</param>
        /// <param name="agent">The tenant agent.</param>
        public TenantConfigurationNode(IPowerShellVfs vfs, ITenantAgent agent)
            : base(vfs, "Settings")
        {
            this.agent = agent;
        }

        /// <summary>
        /// Gets the name of the tenant.
        /// </summary>
        private string TenantName
        {
            get
            {
                return this.GetCurrentTenantName();
            }
        }

        /// <summary>
        /// Gets the keys of the property store.
        /// </summary>
        /// <returns>A sequence of key names.</returns>
        protected override IEnumerable<string> GetKeys()
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (this.keys == null)
            {
                this.keys = this.agent.GetTenantSettingNames().ToArray();
            }

            return this.keys;
        }

        /// <summary>
        /// Gets the value of the specified key.
        /// </summary>
        /// <param name="name">The name of the key.</param>
        /// <returns>The value of the specified key.</returns>
        protected override object GetValueInternal(string name)
        {
            return this.agent.GetTenantSetting(this.TenantName, name);
        }

        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <param name="name">The name of the key to set.</param>
        /// <param name="value">The value to set.</param>
        protected override void SetValueInternal(string name, object value)
        {
            this.agent.UpdateTenantSetting(this.TenantName, name, value);
        }
    }
}