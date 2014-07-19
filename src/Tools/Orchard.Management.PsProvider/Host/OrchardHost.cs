namespace Orchard.Management.PsProvider.Host 
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Runtime.Remoting.Lifetime;
    using System.Security;
    using System.Web.Hosting;

    /// <summary>
    /// Implements the object which is used to create a session between the PowerShell AppDomain and Orchard web
    /// application's AppDomain.
    /// </summary>
    public class OrchardHost : MarshalByRefObject, IRegisteredObject 
    {
        /// <summary>
        /// The host agent instance.
        /// </summary>
        private object agent;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardHost"/> class.
        /// </summary>
        public OrchardHost() 
        {
            HostingEnvironment.RegisterObject(this);
        }

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

        /// <summary>
        /// Starts the Orchard session.
        /// </summary>
        /// <returns>Return code.</returns>
        public ReturnCode StartSession() 
        {
            this.agent = Activator.CreateInstance("Orchard.Framework", "Orchard.Commands.CommandHostAgent").Unwrap();
            
            MethodInfo methodInfo = this.agent.GetType().GetMethod("StartHost");
            return (ReturnCode)methodInfo.Invoke(this.agent, new object[] { TextReader.Null, TextWriter.Null });
        }

        /// <summary>
        /// Stops the current Orchard session.
        /// </summary>
        public void StopSession() 
        {
            if (this.agent != null) 
            {
                MethodInfo methodInfo = this.agent.GetType().GetMethod("StopHost");
                methodInfo.Invoke(this.agent, new object[] { TextReader.Null, TextWriter.Null });
                this.agent = null;
            }
        }
    }
}