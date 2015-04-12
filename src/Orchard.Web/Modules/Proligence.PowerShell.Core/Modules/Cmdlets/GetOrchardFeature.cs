namespace Proligence.PowerShell.Core.Modules.Cmdlets
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management.Automation;
    using Orchard.Environment.Descriptor.Models;
    using Orchard.Environment.Extensions;
    using Orchard.Environment.Extensions.Models;
    using Proligence.PowerShell.Provider;

    [Cmdlet(VerbsCommon.Get, "OrchardFeature", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardFeature : RetrieveOrchardFeatureCmdletBase<FeatureDescriptor>
    {
        protected override IEnumerable<FeatureDescriptor> GetFeatures(string tenant)
        {
            return this.UsingWorkContextScope(
                tenant, 
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

        protected override bool IsFeatureEnabled(FeatureDescriptor feature, string tenant)
        {
            return this.UsingWorkContextScope(
                tenant,
                scope =>
                {
                    var shellDescriptor = scope.Resolve<ShellDescriptor>();
                    var enabledFeatures = scope.Resolve<IExtensionManager>().EnabledFeatures(shellDescriptor);
                    return enabledFeatures.Any(f => f.Id == feature.Id);
                });
        }
    }
}