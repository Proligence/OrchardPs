namespace Orchard.Management.PsProvider.Agents
{
    using System.Diagnostics.CodeAnalysis;
    using System.Web.Hosting;

    /// <summary>
    /// The base class for classes which implement agents which expose features outside Orchard's web application
    /// AppDomain.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "By design.")]
    public interface IAgent : IRegisteredObject
    {
    }
}