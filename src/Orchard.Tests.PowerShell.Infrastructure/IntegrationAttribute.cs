using System;
using Xunit.Sdk;

namespace Orchard.Tests.PowerShell.Infrastructure {
    [AttributeUsage(AttributeTargets.Method)]
    [TraitDiscoverer("Orchard.Tests.Modules.PowerShell.Core.Infrastructure.IntegrationTraitDiscoverer", "Orchard.Tests.Modules.PowerShell.Core")]
    public class IntegrationAttribute : Attribute, ITraitAttribute {
    }
}