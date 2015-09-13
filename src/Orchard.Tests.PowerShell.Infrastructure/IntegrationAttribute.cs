using System;
using Xunit.Sdk;

namespace Orchard.Tests.PowerShell.Infrastructure {
    [AttributeUsage(AttributeTargets.Method)]
    [TraitDiscoverer("Orchard.Tests.PowerShell.Infrastructure.IntegrationTraitDiscoverer", "Orchard.Tests.PowerShell.Infrastructure")]
    public class IntegrationAttribute : Attribute, ITraitAttribute {
    }
}