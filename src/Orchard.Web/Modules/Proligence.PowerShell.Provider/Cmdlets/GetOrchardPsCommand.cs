using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Proligence.PowerShell.Provider.Utilities;
using Proligence.PowerShell.Provider.Vfs.Navigation;

namespace Proligence.PowerShell.Provider.Cmdlets {
    [CmdletAlias("gopc")]
    [Cmdlet(VerbsCommon.Get, "OrchardPsCommand", DefaultParameterSetName = "Default", SupportsShouldProcess = false, ConfirmImpact = ConfirmImpact.None)]
    public class GetOrchardPsCommand : OrchardCmdlet {
        /// <summary>
        /// Gets or sets a value indicating whether all Orchard-related cmdlets should be returned.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Default")]
        public SwitchParameter All { get; set; }

        /// <summary>
        /// Gets or sets the full or partial name of the cmdlets to filter.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Default", Position = 1)]
        [Parameter(Mandatory = false, ParameterSetName = "Path", Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of the item for which supported cmdlets should be returned.
        /// </summary>
        [Parameter(Mandatory = false, ParameterSetName = "Path", Position = 2)]
        public string Path { get; set; }

        protected override void ProcessRecord() {
            CmdletInfo[] cmdlets;

            if (All) {
                cmdlets = GetAllCmdlets();
            }
            else {
                cmdlets = GetCmdletsForCurrentLocation();

                if ((Path == null) && (cmdlets.Length == 0)) {
                    cmdlets = GetAllCmdlets();
                }
            }

            if (Name != null) {
                cmdlets = cmdlets.Where(c => c.Name.WildcardEquals(Name)).ToArray();
            }

            foreach (CmdletInfo cmdlet in cmdlets.OrderBy(c => c.Name)) {
                WriteObject(cmdlet);
            }
        }

        private CmdletInfo[] GetCmdletsForCurrentLocation() {
            var results = new List<CmdletInfo>();

            VfsNode node;
            if (string.IsNullOrEmpty(Path)) {
                node = CurrentNode;
            }
            else {
                Path = Path.TrimStart('.', '\\');
                node = CurrentNode.Vfs.NavigatePath(Path);

                if (node == null) {
                    WriteError(Error.InvalidOperation(
                        "Failed to find item '" + Path + "'.",
                        ErrorIds.FailedToFindItem,
                        Path));
                }
            }

            if (node != null) {
                Attribute[] attributes = Attribute.GetCustomAttributes(
                    node.GetType(),
                    typeof (SupportedCmdletAttribute));

                results.AddRange(
                    attributes
                        .Cast<SupportedCmdletAttribute>()
                        .Select(attribute => InvokeCommand.GetCmdlet(attribute.CmdletName))
                        .Where(cmdletInfo => cmdletInfo != null));
            }

            return results.ToArray();
        }

        private CmdletInfo[] GetAllCmdlets() {
            return InvokeCommand
                .GetCmdlets()
                .Where(cmdletInfo => cmdletInfo.ImplementingType.GetInterfaces().Any(t => t == typeof (IOrchardCmdlet)))
                .ToArray();
        }
    }
}