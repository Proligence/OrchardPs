namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Environment.Features;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("dof")]
    [Cmdlet(VerbsLifecycle.Disable, "OrchardFeature", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class DisableOrchardFeature : AlterOrchardFeatureCmdletBase<FeatureDescriptor>
    {
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "FeatureObject", Mandatory = false)]
        [Parameter(ParameterSetName = "TenantObject", Mandatory = true, Position = 1)]
        [Parameter(ParameterSetName = "AllTenants", Mandatory = true, Position = 1)]
        public SwitchParameter WithoutDependencies { get; set; }

        protected override IEnumerable<FeatureDescriptor> GetTenantFeatures(string tenantName)
        {
            return this.UsingWorkContextScope(
                tenantName,
                scope =>
                {
                    var extensionManager = scope.Resolve<IExtensionManager>();
                    return extensionManager.AvailableFeatures().ToArray();
                });
        }

        protected override string GetFeatureId(FeatureDescriptor feature)
        {
            return feature.Id;
        }

        protected override string GetActionName(FeatureDescriptor feature, string tenantName)
        {
            return "Disable Feature";
        }

        protected override void PerformAction(FeatureDescriptor feature, string tenantName)
        {
            this.UsingWorkContextScope(
                tenantName,
                scope =>
                {
                    this.Resolve<IFeatureManager>().DisableFeatures(
                        new[] { feature.Id },
                        !this.WithoutDependencies.ToBool());
                });
        }
    }
}