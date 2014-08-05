namespace Proligence.PowerShell.Commands.NavigationProviders 
{
    using Proligence.PowerShell.Commands.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class CommandsPsNavigationProvider : PsNavigationProvider
    {
        public CommandsPsNavigationProvider()
            : base(NodeType.Tenant)
        {
        }

        public override void Initialize()
        {
            this.Node = new CommandsNode(this.Vfs);
        }
    }
}