namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Descriptor.Models;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Proligence.PowerShell.Provider;

    [CmdletAlias("gof")]
    [Cmdlet(VerbsCommon.Get, "OrchardFeature", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardFeature : RetrieveOrchardFeatureCmdletBase<FeatureDescriptor>
    {
        private IDictionary<string, string[]> enabledTenantFeatures;

        protected override void BeginProcessing()
        {
            base.BeginProcessing();

            this.enabledTenantFeatures = this.Tenants.ToDictionary(
                tenant => tenant.Name,
                tenant => this.UsingWorkContextScope(
                    tenant.Name,
                    scope =>
                    {
                        var shellDescriptor = scope.Resolve<ShellDescriptor>();
                        return scope.Resolve<IExtensionManager>()
                            .EnabledFeatures(shellDescriptor)
                            .Select(f => f.Id)
                            .Distinct()
                            .ToArray();
                    }));
        }

        protected override IEnumerable<FeatureDescriptor> GetFeatures(string tenantName)
        {
            return this.UsingWorkContextScope(
                tenantName, 
                scope => scope.Resolve<IExtensionManager>().AvailableFeatures());
        }

        protected override string GetFeatureId(FeatureDescriptor feature)
        {
            return feature.Id;
        }

        protected override string GetFeatureName(FeatureDescriptor feature)
        {
            return feature.Name;
        }

        protected override bool IsFeatureEnabled(FeatureDescriptor feature, string tenantName)
        {
            if (this.enabledTenantFeatures.ContainsKey(tenantName))
            {
                if (this.enabledTenantFeatures[tenantName].Contains(feature.Id))
                {
                    return true;
                }
            }

            return false;
        }
    }
}