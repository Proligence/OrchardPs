// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AgentProxy.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Agents 
{
    using System;
    using System.Linq;
    using System.Runtime.Remoting.Lifetime;
    using System.Security;
    using System.Web.Hosting;

    /// <summary>
    /// The base class for classes which implement agent proxies.
    /// </summary>
    public abstract class AgentProxy : MarshalByRefObject, IRegisteredObject 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AgentProxy"/> class.
        /// </summary>
        protected AgentProxy() 
        {
            HostingEnvironment.RegisterObject(this);

            var agentAttribute = GetType().GetCustomAttributes(typeof(AgentAttribute), false)
                .Cast<AgentAttribute>()
                .SingleOrDefault();

            if (agentAttribute == null) 
            {
                throw new OrchardProviderException(
                    "The agent class '" + GetType().FullName + "' must be decorated with AgentAttribute.");
            }

            Type agentType =
                AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                    assembly => assembly.GetTypes().Where(
                        type => type.AssemblyQualifiedName == agentAttribute.TypeName)).SingleOrDefault();

            if (agentType == null) 
            {
                throw new OrchardProviderException(
                    "Failed to instantiate agent because the type '" + agentAttribute.TypeName + "' was not found.");
            }

            this.Agent = Activator.CreateInstance(agentType);
        }

        /// <summary>
        /// Gets the instance of the agent.
        /// </summary>
        public object Agent { get; private set; }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="ILease"/> used to control the lifetime policy for this instance. This is the
        /// current lifetime service object for this instance if one exists; otherwise, a new lifetime service object
        /// initialized to the value of the <see cref="LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        [SecurityCritical]
        public override object InitializeLifetimeService() 
        {
            // never expire the license
            return null;
        }

        /// <summary>
        /// Requests a registered object to unregister.
        /// </summary>
        /// <param name="immediate">
        /// <c>true</c> to indicate the registered object should unregister from the hosting environment before
        /// returning; otherwise, <c>false</c>.
        /// </param>
        [SecuritySafeCritical]
        public void Stop(bool immediate)
        {
            HostingEnvironment.UnregisterObject(this);
        }

        /// <summary>
        /// Invokes the specified method on the agent instance.
        /// </summary>
        /// <param name="methodName">The name of the method to invoke.</param>
        /// <param name="parameters">The method's parameters.</param>
        /// <returns>The method's result.</returns>
        public object Invoke(string methodName, params object[] parameters) 
        {
            return this.Agent
                .GetType()
                .GetMethod(methodName)
                .Invoke(this.Agent, parameters);
        }
    }
}