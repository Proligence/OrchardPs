namespace Proligence.PowerShell.Agents 
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Orchard;
    using Orchard.Environment.Configuration;
    using Orchard.FileSystems.AppData;
    using Orchard.Settings;

    /// <summary>
    /// Implements the the agent which exposes Orchard tenants.
    /// </summary>
    public class TenantAgent : ITenantAgent
    {
        private readonly IShellSettingsManager manager;
        private readonly IAppDataFolder appDataFolder;
        private readonly IOrchardServices services;

        public TenantAgent(IShellSettingsManager manager, IAppDataFolder appDataFolder, IOrchardServices services) 
        {
            this.manager = manager;
            this.appDataFolder = appDataFolder;
            this.services = services;
        }

        /// <summary>
        /// Gets the tenants configured in the Orchard installation.
        /// </summary>
        /// <returns>
        /// An array of <see cref="ShellSettings"/> objects which represent the tenants configured in the Orchard
        /// installation.
        /// </returns>
        public ShellSettings[] GetTenants() 
        {
            return manager.LoadSettings().ToArray();
        }

        /// <summary>
        /// Gets the tenant with the specified name.
        /// </summary>
        /// <param name="name">The name of the tenant to get.</param>
        /// <returns>
        /// A <see cref="ShellSettings"/> object which represent the tenant with the specified name or <c>null</c>.
        /// </returns>
        public ShellSettings GetTenant(string name)
        {
            return manager.LoadSettings().FirstOrDefault(t => t.Name == name);
        }

        /// <summary>
        /// Enables the tenant with the specified name.
        /// </summary>
        /// <param name="name">The name of the tenant to enable.</param>
        public void EnableTenant(string name)
        {
            ShellSettings tenant = manager.LoadSettings().FirstOrDefault(x => x.Name == name);
            if (tenant == null)
            {
                throw new ArgumentException("Failed to find tenant '" + name + "'.");
            }

            if (tenant.Name == ShellSettings.DefaultName)
            {
                throw new InvalidOperationException("Cannot enable default tenant.");
            }

            if (tenant.State != TenantState.Running)
            {
                tenant.State = TenantState.Running;
                manager.SaveSettings(tenant);
            }
        }

        /// <summary>
        /// Disables the tenant with the specified name.
        /// </summary>
        /// <param name="name">The name of the tenant to disable.</param>
        public void DisableTenant(string name)
        {
            ShellSettings tenant = manager.LoadSettings().FirstOrDefault(x => x.Name == name);
            if (tenant == null)
            {
                throw new ArgumentException("Failed to find tenant '" + name + "'.");
            }

            if (tenant.Name == ShellSettings.DefaultName)
            {
                throw new InvalidOperationException("Cannot disable default tenant.");
            }

            if (tenant.State != TenantState.Disabled)
            {
                tenant.State = TenantState.Disabled;
                manager.SaveSettings(tenant);
            }
        }

        /// <summary>
        /// Creates a new tenant.
        /// </summary>
        /// <param name="tenant">The new tenant to create.</param>
        public void CreateTenant(ShellSettings tenant) 
        {
            if (tenant == null)
            {
                throw new ArgumentNullException("tenant");
            }

            if (!string.IsNullOrEmpty(tenant.Name) && !Regex.IsMatch(tenant.Name, @"^\w+$"))
            {
                throw new InvalidOperationException(
                    "Invalid tenant name. Must contain characters only and no spaces.");
            }

            if (tenant.Name == ShellSettings.DefaultName)
            {
                throw new InvalidOperationException("Invalid tenant name.");
            }

            ShellSettings defaultTenant = manager.LoadSettings().FirstOrDefault(
                x => x.Name == ShellSettings.DefaultName);

            if (defaultTenant == null)
            {
                throw new InvalidOperationException("Failed to find default tenant.");
            }

            tenant.State = TenantState.Uninitialized;
            tenant.Themes = defaultTenant.Themes;
            tenant.Modules = defaultTenant.Modules;

            manager.SaveSettings(tenant);
        }

        /// <summary>
        /// Updates an existing tenant.
        /// </summary>
        /// <param name="tenant">The updated tenant.</param>
        public void UpdateTenant(ShellSettings tenant)
        {
            if (tenant == null)
            {
                throw new ArgumentNullException("tenant");
            }

            ShellSettings settings = manager.LoadSettings().FirstOrDefault(x => x.Name == tenant.Name);
            if (settings == null)
            {
                throw new ArgumentException("Failed to find tenant '" + tenant.Name + "'.", "tenant");
            }

            settings.RequestUrlHost = tenant.RequestUrlHost;
            settings.RequestUrlPrefix = tenant.RequestUrlPrefix;
            settings.DataProvider = tenant.DataProvider;
            settings.DataConnectionString = tenant.DataConnectionString;
            settings.DataTablePrefix = tenant.DataTablePrefix;
            manager.SaveSettings(settings);
        }

        /// <summary>
        /// Removes an existing tenant.
        /// </summary>
        /// <param name="tenantName">The name of the tenant to remove.</param>
        public void RemoveTenant(string tenantName)
        {
            if (string.IsNullOrEmpty(tenantName))
            {
                throw new ArgumentNullException("tenantName");
            }

            if (tenantName == ShellSettings.DefaultName)
            {
                throw new InvalidOperationException("Cannot remove default tenant.");
            }

            var settings = manager.LoadSettings().FirstOrDefault(x => x.Name == tenantName);
            if (settings == null)
            {
                throw new ArgumentException("Failed to find tenant '" + tenantName + "'.", "tenantName");
            }

            string filePath = Path.Combine(Path.Combine("Sites", settings.Name), "Settings.txt");
            appDataFolder.DeleteFile(filePath);
        }

        /// <summary>
        /// Gets the names of all supported tenant settings.
        /// </summary>
        /// <returns>A sequence of setting names.</returns>
        public IEnumerable<string> GetTenantSettingNames()
        {
            return typeof(ISite).GetProperties().Select(p => p.Name).ToArray();
        }

        /// <summary>
        /// Gets the value of the specified tenant setting.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="settingName">The name of the setting to get.</param>
        /// <returns>The value of the tenant setting.</returns>
        public object GetTenantSetting(string tenantName, string settingName)
        {
            ISite currentSite = services.WorkContext.CurrentSite;

            PropertyInfo property = currentSite.GetType().GetProperty(settingName);
            if (property == null)
            {
                throw new ArgumentException("Invalid tenant setting: " + settingName, "settingName");
            }

            return property.GetValue(currentSite);
        }

        /// <summary>
        /// Updates the value of the specified tenant setting.
        /// </summary>
        /// <param name="tenantName">The name of the tenant.</param>
        /// <param name="settingName">The name of the setting to update.</param>
        /// <param name="value">The new value for the tenant setting.</param>
        public void UpdateTenantSetting(string tenantName, string settingName, object value)
        {
            ISite currentSite = services.WorkContext.CurrentSite;
                
            PropertyInfo property = currentSite.GetType().GetProperty(settingName);
            if (property == null)
            {
                throw new ArgumentException("Invalid tenant setting: " + settingName, "settingName");
            }

            property.SetValue(currentSite, value);
        }
    }
}