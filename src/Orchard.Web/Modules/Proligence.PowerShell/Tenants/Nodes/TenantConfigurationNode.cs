namespace Proligence.PowerShell.Tenants.Nodes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Orchard.Settings;
    using Proligence.PowerShell.Common.Items;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Vfs;
    using Proligence.PowerShell.Provider.Vfs.Navigation;
    using Proligence.PowerShell.Utilities;

    /// <summary>
    /// Implements a VFS node which represents the configuration of an Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Set-Item")]
    public class TenantConfigurationNode : CachedPropertyStoreNode
    {
        /// <summary>
        /// Contains cached tenant setting names.
        /// </summary>
        private string[] keys;

        public TenantConfigurationNode(IPowerShellVfs vfs)
            : base(vfs, "Settings")
        {
            this.Item = new DescriptionItem
            {
                Name = "Settings",
                Description = "Contains the settings of the current tenant."
            };
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
        public override IEnumerable<string> GetKeys()
        {
            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (this.keys == null)
            {
                this.keys = GetTenantSettingNames();
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
            return this.UsingWorkContextScope(
                this.TenantName,
                scope =>
                    {
                        ISite currentSite = scope.WorkContext.CurrentSite;

                        PropertyInfo property = currentSite.GetType().GetProperty(name);
                        if (property == null)
                        {
                            throw new ArgumentException("Invalid tenant setting: " + name, "name");
                        }

                        return property.GetValue(currentSite);
                    });
        }

        /// <summary>
        /// Sets the value of the specified key.
        /// </summary>
        /// <param name="name">The name of the key to set.</param>
        /// <param name="value">The value to set.</param>
        protected override void SetValueInternal(string name, object value)
        {
            this.UsingWorkContextScope(
                this.TenantName,
                scope =>
                    {
                        ISite currentSite = scope.WorkContext.CurrentSite;

                        PropertyInfo property = currentSite.GetType().GetProperty(name);
                        if (property == null)
                        {
                            throw new ArgumentException("Invalid tenant setting: " + name, "name");
                        }

                        property.SetValue(currentSite, value);
                    });
        }

        private static string[] GetTenantSettingNames()
        {
            return typeof(ISite).GetProperties().Select(p => p.Name).ToArray();
        }
    }
}