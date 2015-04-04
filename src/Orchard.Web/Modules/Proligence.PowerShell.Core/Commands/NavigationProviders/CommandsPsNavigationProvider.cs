namespace Proligence.PowerShell.Core.Commands.NavigationProviders 
{
    using Proligence.PowerShell.Core.Commands.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class CommandsPsNavigationProvider : PsNavigationProvider
    {
        public CommandsPsNavigationProvider()
            : base(NodeType.Tenant)
        {
        }

        protected override void InitializeInternal()
        {
            this.Node = new CommandsNode(this.Vfs);
        }
    }
}