using System;
using System.Linq;
using System.Security;
using System.Web.Hosting;

namespace Orchard.Management.PsProvider.Agents {
    public abstract class AgentProxy : MarshalByRefObject, IRegisteredObject {
        public object Agent { get; private set; }
        
        protected AgentProxy() {
            HostingEnvironment.RegisterObject(this);

            var agentAttribute = GetType().GetCustomAttributes(typeof(AgentAttribute), false)
                .Cast<AgentAttribute>()
                .SingleOrDefault();

            if (agentAttribute == null) {
                throw new OrchardProviderException(
                    "The agent class '" + GetType().FullName + "' must be decorated with AgentAttribute.");
            }

            Type agentType =
                (AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                assembly => assembly.GetTypes().Where(
                    type => type.AssemblyQualifiedName == agentAttribute.TypeName))).SingleOrDefault();

            if (agentType == null) {
                throw new OrchardProviderException("Failed to instantiate agent.");
            }

            Agent = Activator.CreateInstance(agentType);
        }

        [SecurityCritical]
        public override object InitializeLifetimeService() {
            // never expire the license
            return null;
        }

        [SecuritySafeCritical]
        void IRegisteredObject.Stop(bool immediate) {
            HostingEnvironment.UnregisterObject(this);
        }

        public object Invoke(string methodName, params object[] parameters) {
            return Agent.GetType().GetMethod(methodName).Invoke(Agent, parameters);
        }
    }
}