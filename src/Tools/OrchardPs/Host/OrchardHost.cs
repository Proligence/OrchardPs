namespace OrchardPs.Host
{
    using System;
    using System.Reflection;
    using System.Runtime.Remoting.Lifetime;
    using System.Security;
    using System.Web.Hosting;
    using Proligence.PowerShell.Provider;

    /// <summary>
    /// Implements the object which is used to create a session between the PowerShell AppDomain and Orchard web
    /// application's AppDomain.
    /// </summary>
    public class OrchardHost : MarshalByRefObject, IRegisteredObject 
    {
        private object agent;
        private IPsSession session;
        private IPsSessionManager sessionManager;
        private string connectionId;

        public OrchardHost()
        {
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
        public override object InitializeLifetimeService() 
        {
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

        public IPsSession StartSession(IConsoleConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            this.Connection = connection;

            this.agent = Activator.CreateInstance(
                "Proligence.PowerShell.Provider",
                "Proligence.PowerShell.Provider.PsProviderAgent")
                .Unwrap();

            MethodInfo methodInfo = this.agent.GetType().GetMethod("GetSessionManager");
            this.sessionManager = (IPsSessionManager)methodInfo.Invoke(this.agent, new object[0]);
            this.connectionId = Guid.NewGuid().ToString();
            this.Connection = this.Connection;
            this.session = this.sessionManager.NewSession(this.connectionId, this.Connection);

            return this.session;
        }

        public void StopSession() 
        {
            if (this.agent != null) 
            {
                if (this.session != null)
                {
                    this.sessionManager.CloseSession(this.session);
                    this.session = null;
                }

                if (this.agent != null)
                {
                    MethodInfo methodInfo = this.agent.GetType().GetMethod("Dispose");
                    methodInfo.Invoke(this.agent, new object[0]);
                    this.agent = null;
                }
            }
        }
    }
}