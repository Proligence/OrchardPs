using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Web.Hosting;

namespace Orchard.Management.PsProvider.Host {
    public class OrchardHost : MarshalByRefObject, IRegisteredObject {
        private object _agent;

        public OrchardHost() {
            HostingEnvironment.RegisterObject(this);
        }

        [SecurityCritical]
        public override object InitializeLifetimeService() {
            return null;
        }

        [SecuritySafeCritical]
        void IRegisteredObject.Stop(bool immediate) {
            HostingEnvironment.UnregisterObject(this);
        }

        public ReturnCodes StartSession() {
            _agent = Activator.CreateInstance("Orchard.Framework", "Orchard.Commands.CommandHostAgent").Unwrap();
            
            MethodInfo methodInfo = _agent.GetType().GetMethod("StartHost");
            return (ReturnCodes)methodInfo.Invoke(_agent, new object[] { TextReader.Null, TextWriter.Null });
        }

        public void StopSession() {
            if (_agent != null) {
                MethodInfo methodInfo = _agent.GetType().GetMethod("StopHost");
                methodInfo.Invoke(_agent, new object[] { TextReader.Null, TextWriter.Null });
                _agent = null;
            }
        }
    }
}