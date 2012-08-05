// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardSession.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider.Host 
{
    using Orchard.Management.PsProvider.Agents;

    /// <summary>
    /// Represents the connection with the AppDomain of the Orchard web application.
    /// </summary>
    internal class OrchardSession : IOrchardSession 
    {
        private readonly OrchardHostContextProvider hostContextProvider;
        private OrchardHostContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrchardSession"/> class.
        /// </summary>
        /// <param name="orchardPath">The path to the root folder of the Orchard installation.</param>
        public OrchardSession(string orchardPath) 
        {
            this.hostContextProvider = new OrchardHostContextProvider(orchardPath);
        }

        /// <summary>
        /// Initializes the Orchard session.
        /// </summary>
        public void Initialize() 
        {
            OrchardHostContext ctx = this.hostContextProvider.CreateContext();
            if (ctx.StartSessionResult == ctx.RetryResult) 
            {
                ctx = this.hostContextProvider.CreateContext();
            }
            else if (ctx.StartSessionResult == ReturnCode.Fail) 
            {
                this.hostContextProvider.Shutdown(this.context);
                throw new OrchardProviderException("Failed to initialize Orchard session.");
            }
            
            this.context = ctx;
        }

        /// <summary>
        /// Shuts down the Orchard session.
        /// </summary>
        public void Shutdown() 
        {
            this.hostContextProvider.Shutdown(this.context);
        }

        /// <summary>
        /// Creates a proxy for an agent of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the agent to create.</typeparam>
        /// <returns>The created agent proxy instance.</returns>
        public T CreateAgent<T>() 
            where T : AgentProxy 
        {
            return this.hostContextProvider.CreateAgent<T>();
        }
    }
}