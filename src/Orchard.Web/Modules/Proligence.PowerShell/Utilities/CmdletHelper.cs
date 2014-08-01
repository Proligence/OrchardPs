namespace Proligence.PowerShell.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;

    using Orchard.Environment.Configuration;

    using Proligence.PowerShell.Common.Extensions;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements helper methods for implementing cmdlets.
    /// </summary>
    public static class CmdletHelper
    {
        /// <summary>
        /// Filters a list of Orchard tenants by the standard criteria used by cmdlets.
        /// </summary>
        /// <param name="cmdlet">The cmdlet which is calling this method.</param>
        /// <param name="tenants">A list of tenants to filter.</param>
        /// <param name="allTenants">If set to <c>true</c> all tenants will be returned.</param>
        /// <param name="tenantName">The name of the tenants to filter.</param>
        /// <param name="tenant">The tenant to filter.</param>
        /// <returns>A sequence of filtered <see cref="ShellSettings"/> objects.</returns>
        public static IEnumerable<ShellSettings> FilterTenants(
            OrchardCmdlet cmdlet,
            IEnumerable<ShellSettings> tenants,
            bool allTenants,
            string tenantName,
            ShellSettings tenant)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            if (tenants == null)
            {
                throw new ArgumentNullException("tenants");
            }

            var result = new List<ShellSettings>();

            if (allTenants)
            {
                result.AddRange(tenants);
            }
            else if (!string.IsNullOrEmpty(tenantName))
            {
                ShellSettings[] namedTenants = tenants
                    .Where(s => s.Name.Equals(tenantName, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (namedTenants.Any())
                {
                    result.AddRange(namedTenants);
                }
                else
                {
                    var ex = new ArgumentException("Failed to find tenant with name '" + tenantName + "'.");
                    cmdlet.WriteError(ex, "FailedToFindTenant", ErrorCategory.InvalidArgument);
                }
            }
            else if (tenant != null)
            {
                result.Add(tenant);
            }
            else
            {
                ShellSettings currentTenant = cmdlet.GetCurrentTenant();
                if (currentTenant != null)
                {
                    result.Add(currentTenant);
                }
                else
                {
                    ShellSettings defaultTenant = tenants.SingleOrDefault(
                        s => s.Name.Equals("Default", StringComparison.OrdinalIgnoreCase));

                    if (defaultTenant != null)
                    {
                        result.Add(defaultTenant);
                    }
                    else
                    {
                        var ex = new ArgumentException("Failed to find tenant with name 'Default'.");
                        cmdlet.WriteError(ex, "FailedToFindTenant", ErrorCategory.InvalidArgument);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Filters a list of Orchard tenants by the standard criteria used by cmdlets.
        /// </summary>
        /// <param name="cmdlet">The cmdlet which is calling this method.</param>
        /// <param name="tenants">A list of tenants to filter.</param>
        /// <returns>A sequence of filtered <see cref="ShellSettings"/> objects.</returns>
        public static IEnumerable<ShellSettings> FilterTenants(
            ITenantFilterCmdlet cmdlet,
            IEnumerable<ShellSettings> tenants)
        {
            if (cmdlet == null)
            {
                throw new ArgumentNullException("cmdlet");
            }

            var orchardCmdlet = cmdlet as OrchardCmdlet;
            if (orchardCmdlet == null)
            {
                throw new ArgumentException("The specified object is not an Orchard cmdlet.");
            }

            return FilterTenants(
                orchardCmdlet,
                tenants,
                cmdlet.FromAllTenants,
                cmdlet.Tenant,
                cmdlet.TenantObject);
        }
    }
}