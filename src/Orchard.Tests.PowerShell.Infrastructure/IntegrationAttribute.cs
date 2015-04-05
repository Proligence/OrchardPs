namespace Orchard.Tests.PowerShell.Infrastructure
{
    using System;
    using Xunit.Sdk;

    [AttributeUsage(AttributeTargets.Method)]
    [TraitDiscoverer("Orchard.Tests.Modules.PowerShell.Core.Infrastructure.IntegrationTraitDiscoverer", "Orchard.Tests.Modules.PowerShell.Core")]
    public class IntegrationAttribute : Attribute, ITraitAttribute
    {
    }
}