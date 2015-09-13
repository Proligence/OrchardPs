using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orchard.Settings;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Vfs;
using Proligence.PowerShell.Provider.Vfs.Items;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Core.Tenants.Nodes {
    /// <summary>
    /// Implements a VFS node which represents the configuration of an Orchard tenant.
    /// </summary>
    [SupportedCmdlet("Set-Item")]
    public class TenantConfigurationNode : CachedPropertyStoreNode {
        /// <summary>
        /// Contains cached tenant setting names.
        /// </summary>
        private string[] _keys;

        public TenantConfigurationNode(IPowerShellVfs vfs)
            : base(vfs, "Settings") {
            Item = new DescriptionItem {
                Name = "Settings",
                Description = "Contains the settings of the current tenant."
            };
        }

        private string TenantName {
            get { return this.GetCurrentTenantName(); }
        }

        public override IEnumerable<string> GetKeys() {
            // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
            if (_keys == null) {
                _keys = GetTenantSettingNames();
            }

            return _keys;
        }

        protected override object GetValueInternal(string name) {
            return this.UsingWorkContextScope(
                TenantName,
                scope => {
                    ISite currentSite = scope.WorkContext.CurrentSite;

                    PropertyInfo property = currentSite.GetType().GetProperty(name);
                    if (property == null) {
                        throw new ArgumentException("Invalid tenant setting: " + name, "name");
                    }

                    return property.GetValue(currentSite);
                });
        }

        protected override void SetValueInternal(string name, object value) {
            this.UsingWorkContextScope(
                TenantName,
                scope => {
                    ISite currentSite = scope.WorkContext.CurrentSite;

                    PropertyInfo property = currentSite.GetType().GetProperty(name);
                    if (property == null) {
                        throw new ArgumentException("Invalid tenant setting: " + name, "name");
                    }

                    property.SetValue(currentSite, value);
                });
        }

        private static string[] GetTenantSettingNames() {
            return typeof (ISite).GetProperties().Select(p => p.Name).ToArray();
        }
    }
}