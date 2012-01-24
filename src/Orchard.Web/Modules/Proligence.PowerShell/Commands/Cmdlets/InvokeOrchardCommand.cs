using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Orchard.Management.PsProvider;
using Proligence.PowerShell.Agents;
using Proligence.PowerShell.Commands.Items;
using Proligence.PowerShell.Common.Extensions;
using Proligence.PowerShell.Sites.Items;

namespace Proligence.PowerShell.Commands.Cmdlets {
    [CmdletAlias("ioc")]
    [Cmdlet(VerbsLifecycle.Invoke, "OrchardCommand", DefaultParameterSetName = "Default", ConfirmImpact = ConfirmImpact.Medium)]
    public class InvokeOrchardCommand : OrchardCmdlet {
        private CommandAgentProxy _commandAgent;

        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "CommandObject", ValueFromPipeline = true)]
        public OrchardCommand Command { get; set; }

        [Parameter(ParameterSetName = "Default", Position = 2, ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "CommandObject", Position = 2, ValueFromRemainingArguments = true)]
        public ArrayList Parameters { get; set; }

        protected override void BeginProcessing() {
            base.BeginProcessing();
            _commandAgent = AgentManager.GetAgent<CommandAgentProxy>();
        }

        protected override void ProcessRecord() {
            OrchardSite site = this.GetCurrentSite();
            string siteName = site != null ? site.Name : "Default";

            switch (ParameterSetName) {
                case "Default":
                    InvokeDefault(siteName, Name, Parameters);
                    break;
                case "CommandObject":
                    InvokeCommandObject(siteName, Command, Parameters);
                    break;
            }
        }

        private void InvokeDefault(string siteName, string commandName, ArrayList parameters) {
            var arguments = new List<string>();
            var switches = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(commandName)) {
                arguments.Add(commandName);
            }

            InvokeWithParameters(siteName, arguments, switches, parameters);
        }

        private void InvokeCommandObject(string siteName, OrchardCommand command, ArrayList parameters) {
            var arguments = new List<string>(command.CommandName.Split(new[] { ' ' }));
            var switches = new Dictionary<string, string>();
            InvokeWithParameters(siteName, arguments, switches, parameters);
        }

        private void InvokeWithParameters(string siteName, List<string> arguments, Dictionary<string, string> switches, ArrayList parameters) {
            if (parameters != null) {
                IList<string> parsedArgs;
                IDictionary<string, string> parsedSwitches;
                if (ParseParameters(Parameters.ToArray().Cast<string>(), out parsedArgs, out parsedSwitches)) {
                    arguments.AddRange(parsedArgs);
                    foreach (KeyValuePair<string, string> sw in parsedSwitches) {
                        switches.Add(sw.Key, sw.Value);
                    }
                }
                else {
                    return;
                }
            }
                
            _commandAgent.ExecuteCommand(
                siteName, 
                arguments.ToArray(), 
                new Dictionary<string, string>(switches));
        }

        private bool ParseParameters(IEnumerable<string> args, out IList<string> arguments, out IDictionary<string, string> switches) {
            arguments = new List<string>();
            switches = new Dictionary<string, string>();

            foreach (var arg in args) {
                if (arg[0] == '/') {
                    int index = arg.IndexOf(':');
                    var switchName = (index < 0 ? arg.Substring(1) : arg.Substring(1, index - 1));
                    var switchValue = (index < 0 || index >= arg.Length ? string.Empty : arg.Substring(index + 1));

                    if (string.IsNullOrEmpty(switchName)) {
                        ArgumentException exception = new ArgumentException(
                            string.Format("Invalid switch syntax: \"{0}\". Valid syntax is /<switchName>[:<switchValue>].", arg));
                        this.WriteError(exception, "InvalidCommandSwitchSyntax", ErrorCategory.SyntaxError);
                        return false;
                    }

                    switches.Add(switchName, switchValue);
                }
                else {
                    arguments.Add(arg);
                }
            }

            return true;
        }
    }
}