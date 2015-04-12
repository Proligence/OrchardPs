namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Orchard.Environment.Features;
    using Proligence.PowerShell.Provider;

    [Cmdlet(VerbsLifecycle.Enable, "OrchardFeature", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class EnableOrchardFeature : AlterOrchardFeatureCmdletBase<FeatureDescriptor>
    {
        [Parameter(ParameterSetName = "Default", Mandatory = false)]
        [Parameter(ParameterSetName = "FeatureObject", Mandatory = false)]
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
            return "Enable Feature";
        }

        protected override void PerformAction(FeatureDescriptor feature, string tenantName)
        {
            this.UsingWorkContextScope(
                tenantName,
                scope =>
                {
                    this.Resolve<IFeatureManager>().EnableFeatures(
                        new[] { feature.Id },
                        !this.WithoutDependencies.ToBool());
                });
        }
    }
}