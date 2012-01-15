using System.Management.Automation;
using Autofac;
using Orchard.Management.PsProvider.Agents;
using Orchard.Management.PsProvider.Vfs;

namespace Orchard.Management.PsProvider {
    public class OrchardCmdlet : PSCmdlet {
        private IAgentManager _agentManager;
        
        public OrchardDriveInfo OrchardDrive { get; private set; }
        public OrchardVfsNode CurrentNode { get; private set; }
        public IContainer OrchardProviderContainer { get; private set; }
        
        protected IAgentManager AgentManager {
            get {
                if (_agentManager == null) {
                    _agentManager = OrchardDrive.LifetimeScope.Resolve<IAgentManager>();
                }

                return _agentManager;
            }
        }

        protected override void BeginProcessing() {
            OrchardDrive = SessionState.Drive.Current as OrchardDriveInfo;
            if (OrchardDrive == null) {
                var exception = ThrowHelper.InvalidOperation("The cmdlet must be invoked from an Orchard drive.");
                this.ThrowTerminatingError(exception, ErrorIds.OrchardDriveExpected, ErrorCategory.InvalidOperation);
            }

            CurrentNode = OrchardDrive.Vfs.NavigatePath(OrchardDrive.CurrentLocation);
            OrchardProviderContainer = OrchardDrive.GetOrchardProviderContainer();
        }
    }
}