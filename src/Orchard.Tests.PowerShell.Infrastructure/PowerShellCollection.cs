using Xunit;

namespace Orchard.Tests.PowerShell.Infrastructure {
    /// <summary>
    /// This class has no code, and is never created. Its purpose is simply to be the place to apply
    /// <c>[CollectionDefinition]</c> and all the <c>ICollectionFixture{}</c> interfaces.
    /// </summary>
    /// <remarks>
    /// Test which use this collection will share one instance of the PowerShell execution engine created
    /// for unit test purposes.
    /// </remarks>
    [CollectionDefinition("PowerShell")]
    public class PowerShellCollection : ICollectionFixture<PowerShellFixture> {
    }
}