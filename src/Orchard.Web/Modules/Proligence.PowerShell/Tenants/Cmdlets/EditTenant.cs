﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditTenant.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Tenants.Cmdlets
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Management.Automation;
    using Orchard.Management.PsProvider;
    using Proligence.PowerShell.Agents;
    using Proligence.PowerShell.Tenants.Items;

    /// <summary>
    /// Implements the <c>Edit-Tenant</c> cmdlet.
    /// </summary>
    [Cmdlet(VerbsData.Edit, "Tenant", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EditTenant : OrchardCmdlet
    {
        /// <summary>
        /// The tenant agent proxy instance.
        /// </summary>
        private ITenantAgent tenantAgent;

        /// <summary>
        /// Gets or sets the name of the tenant to edit.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the tenant to edit.
        /// </summary>
        [ValidateNotNull]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, ValueFromPipeline = true)]
        public OrchardTenant Tenant { get; set; }

        /// <summary>
        /// Gets or sets the tenant's new request URL host.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design")]
        public string RequestUrlHost { get; set; }

        /// <summary>
        /// Gets or sets the tenant's new request URL prefix.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By design")]
        public string RequestUrlPrefix { get; set; }

        /// <summary>
        /// Gets or sets the new connection string to the tenant's database.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string DataConnectionString { get; set; }

        /// <summary>
        /// Gets or sets the new name of the data provider used by the tenant.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string DataProvider { get; set; }

        /// <summary>
        /// Gets or sets the tenant's new data table prefix.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = false)]
        public string DataTablePrefix { get; set; }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            this.tenantAgent = this.AgentManager.GetAgent<ITenantAgent>();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            string tenantName = null;
            if (this.ParameterSetName == "Default")
            {
                tenantName = this.Name;
            }
            else if (this.ParameterSetName == "TenantObject")
            {
                tenantName = this.Tenant.Name;
            }

            if (tenantName != null)
            {
                OrchardTenant tenant = this.tenantAgent.GetTenant(tenantName);
                if (tenant != null)
                {
                    if (this.RequestUrlHost != null)
                    {
                        tenant.RequestUrlHost = this.RequestUrlHost;
                    }

                    if (this.RequestUrlPrefix != null)
                    {
                        tenant.RequestUrlPrefix = this.RequestUrlPrefix;
                    }

                    if (this.DataConnectionString != null)
                    {
                        tenant.DataConnectionString = this.DataConnectionString;
                    }

                    if (this.DataProvider != null)
                    {
                        tenant.DataProvider = this.DataProvider;
                    }

                    if (this.DataTablePrefix != null)
                    {
                        tenant.DataTablePrefix = this.DataTablePrefix;
                    }

                    this.InvokeUpdateTenant(tenant);
                }
                else
                {
                    var exception = new InvalidOperationException("Failed to find tenant '" + this.Name + "'.");
                    this.WriteError(exception, "FailedToFindTentant", ErrorCategory.InvalidArgument);
                }
            }
            else
            {
                var exception = new InvalidOperationException("Tenant not specified.");
                this.WriteError(exception, "TenantNotSpecified", ErrorCategory.InvalidArgument);
            }
        }

        /// <summary>
        /// Invokes the tenants agent to create a new tenant.
        /// </summary>
        /// <param name="tenant">The tenant to create.</param>
        private void InvokeUpdateTenant(OrchardTenant tenant)
        {
            if (this.ShouldProcess("Tenant: " + tenant.Name, this.GetActionString()))
            {
                try
                {
                    this.tenantAgent.UpdateTenant(tenant);
                }
                catch (ArgumentException ex)
                {
                    this.WriteError(ex, "FailedToUpdateTenant", ErrorCategory.InvalidArgument);
                }
            }
        }

        /// <summary>
        /// Gets a string which describes what changes will be made to the edited tenant.
        /// </summary>
        /// <returns>The generated action string.</returns>
        private string GetActionString()
        {
            var list = new List<string>();
            
            if (this.RequestUrlHost != null)
            {
                list.Add("RequestUrlHost = '" + this.RequestUrlHost + "'");
            }

            if (this.RequestUrlPrefix != null)
            {
                list.Add("RequestUrlPrefix = '" + this.RequestUrlPrefix + "'");
            }

            if (this.DataConnectionString != null)
            {
                list.Add("DataConnectionString = '" + this.DataConnectionString + "'");
            }

            if (this.DataProvider != null)
            {
                list.Add("DataProvider = '" + this.DataProvider + "'");
            }

            if (this.DataTablePrefix != null)
            {
                list.Add("DataTablePrefix = '" + this.DataTablePrefix + "'");
            }

            return "Set " + string.Join(", ", list);
        }
    }
}