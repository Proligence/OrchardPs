using System;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Web.Hosting;
using Proligence.PowerShell.Provider;

namespace OrchardPs.Host {
    /// <summary>
    /// Implements the object which is used to create a session between the PowerShell AppDomain and Orchard web
    /// application's AppDomain.
    /// </summary>
    public class OrchardHost : MarshalByRefObject, IRegisteredObject {
        private object _agent;
        private IPsSession _session;
        private IPsSessionManager _sessionManager;
        private string _connectionId;

        public OrchardHost() {
            HostingEnvironment.RegisterObject(this);
        }

        public IConsoleConnection Connection { get; private set; }

        /// <summary>
        /// Obtains a lifetime service object to control the lifetime policy for this instance.
        /// </summary>
        /// <returns>
        /// An object of type <see cref="ILease"/> used to control the lifetime policy for this instance. This is the
        /// current lifetime service object for this instance if one exists; otherwise, a new lifetime service object
        /// initialized to the value of the <see cref="LifetimeServices.LeaseManagerPollTime"/> property.
        /// </returns>
        [SecurityCritical]
        public override object InitializeLifetimeService() {
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
        public void Stop(bool immediate) {
            HostingEnvironment.UnregisterObject(this);
        }

        public IPsSession StartSession(IConsoleConnection connection) {
            if (connection == null) {
                throw new ArgumentNullException("connection");
            }

            Connection = connection;

            _agent = Activator.CreateInstance(
                "Proligence.PowerShell.Provider",
                "Proligence.PowerShell.Provider.PsProviderAgent")
                .Unwrap();

            MethodInfo methodInfo = _agent.GetType().GetMethod("GetSessionManager");
            _sessionManager = (IPsSessionManager) methodInfo.Invoke(_agent, new object[0]);
            _connectionId = Guid.NewGuid().ToString();
            Connection = Connection;
            _session = _sessionManager.NewSession(_connectionId, Connection);

            return _session;
        }

        public void StopSession() {
            if (_agent != null) {
                if (_session != null) {
                    _sessionManager.CloseSession(_session);
                    _session = null;
                }

                if (_agent != null) {
                    MethodInfo methodInfo = _agent.GetType().GetMethod("Dispose");
                    methodInfo.Invoke(_agent, new object[0]);
                    _agent = null;
                }
            }
        }
    }
}