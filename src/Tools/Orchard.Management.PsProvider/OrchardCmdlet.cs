// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OrchardCmdlet.cs" company="Proligence">
//   Copyright (c) 2011 Proligence, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Orchard.Management.PsProvider 
{
    using System.Management.Automation;
    using Autofac;
    using Orchard.Management.PsProvider.Agents;
    using Orchard.Management.PsProvider.Vfs;

    /// <summary>
    /// The base class for cmdlets which must be invoked from a path which belongs to the Orchard PS provider.
    /// </summary>
    public class OrchardCmdlet : PSCmdlet 
    {
        /// <summary>
        /// The agent manager instance.
        /// </summary>
        private IAgentManager agentManager;
        
        /// <summary>
        /// Gets the <see cref="OrchardDriveInfo"/> object of the Orchard drive on which the cmdlet was called.
        /// </summary>
        public OrchardDriveInfo OrchardDrive { get; private set; }

        /// <summary>
        /// Gets the current VFS node in the Orchard drive on which the cmdlet was called.
        /// </summary>
        public OrchardVfsNode CurrentNode { get; private set; }

        /// <summary>
        /// Gets the dependency injection container for the Orchard PS provider.
        /// </summary>
        public IContainer OrchardProviderContainer { get; private set; }
       
        /// <summary>
        /// Gets the agent manager instance.
        /// </summary>
        protected IAgentManager AgentManager 
        {
            get 
            {
                if (this.agentManager == null) 
                {
                    this.agentManager = this.OrchardDrive.LifetimeScope.Resolve<IAgentManager>();
                }

                return this.agentManager;
            }
        }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing() 
        {
            this.OrchardDrive = SessionState.Drive.Current as OrchardDriveInfo;
            if (this.OrchardDrive == null) 
            {
                var exception = ThrowHelper.InvalidOperation("The cmdlet must be invoked from an Orchard drive.");
                this.ThrowTerminatingError(exception, ErrorIds.OrchardDriveExpected, ErrorCategory.InvalidOperation);
            }

            this.CurrentNode = this.OrchardDrive.Vfs.NavigatePath(this.OrchardDrive.CurrentLocation);
            this.OrchardProviderContainer = this.OrchardDrive.GetOrchardProviderContainer();
        }
    }
}