namespace Proligence.PowerShell.Core.Content.NavigationProviders 
{
    using Proligence.PowerShell.Core.Content.Nodes;
    using Proligence.PowerShell.Provider.Vfs.Navigation;

    public class ContentPsNavigationProvider : PsNavigationProvider
    {
        public ContentPsNavigationProvider()
            : base(NodeType.Tenant)
        {
        }

        protected override void InitializeInternal()
        {
            this.Node = new ContentNode(this.Vfs);
        }
    }
}