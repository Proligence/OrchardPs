namespace Proligence.PowerShell.Cmdlets
{
    using System;
    using System.Linq;
    using System.Management.Automation;
    using Proligence.PowerShell.Internal;
    using Provider;
    using Provider.Vfs.Navigation;

    /// <summary>
    /// Implements the <c>Get-OrchardPsCommand</c> cmdlet.
    /// </summary>
    [CmdletAlias("gopc")]
    [Cmdlet(VerbsCommon.Get, "OrchardPsCommand", DefaultParameterSetName = "Default", SupportsShouldProcess = false, ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardPsCommand : OrchardCmdlet
    {
        /// <summary>
        /// Gets or sets a value indicating whether all Orchard-related cmdlets should be returned.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Default")]
        public SwitchParameter All { get; set; }

        /// <summary>
        /// Gets or sets the path of the item for which supported cmdlets should be returned.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Path", Position = 1)]
        public string Path { get; set; }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord()
        {
            if (this.All)
            {
                foreach (CmdletInfo cmdletInfo in this.InvokeCommand.GetCmdlets())
                {
                    if (cmdletInfo.ImplementingType.GetInterfaces().Any(t => t == typeof(IOrchardCmdlet)))
                    {
                        this.WriteObject(cmdletInfo);
                    }
                }
            }
            else
            {
                VfsNode node;
                
                if (string.IsNullOrEmpty(this.Path))
                {
                    node = this.CurrentNode;
                }
                else
                {
                    this.Path = this.Path.TrimStart('.', '\\');
                    node = this.CurrentNode.Vfs.NavigatePath(this.Path);

                    if (node == null)
                    {
                        var exc = new ArgumentException("Failed to find item '" + this.Path + "'.");
                        this.WriteError(exc, ErrorIds.FailedToFindItem, ErrorCategory.InvalidArgument, this.Path);
                    }
                }

                if (node != null)
                {
                    Attribute[] attributes = Attribute.GetCustomAttributes(
                        node.GetType(),
                        typeof(SupportedCmdletAttribute));

                    foreach (SupportedCmdletAttribute attribute in attributes)
                    {
                        CmdletInfo cmdletInfo = this.InvokeCommand.GetCmdlet(attribute.CmdletName);
                        if (cmdletInfo != null)
                        {
                            this.WriteObject(cmdletInfo);
                        }
                    }
                }
            }
        }
    }
}