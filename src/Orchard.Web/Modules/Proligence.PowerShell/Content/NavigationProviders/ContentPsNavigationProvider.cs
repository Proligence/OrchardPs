namespace Proligence.PowerShell.Content.NavigationProviders 
{
    using Proligence.PowerShell.Content.Nodes;
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