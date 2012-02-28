// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvokeOrchardCommand.cs" company="Proligence">
//   Proligence Confidential, All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Proligence.PowerShell.Commands.Cmdlets 
{
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

    /// <summary>
    /// Implements the <c>Invoke-OrchardCommand</c> cmdlet.
    /// </summary>
    [CmdletAlias("ioc")]
    [Cmdlet(VerbsLifecycle.Invoke, "OrchardCommand", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class InvokeOrchardCommand : OrchardCmdlet
    {
        /// <summary>
        /// The command agent proxy instance.
        /// </summary>
        private CommandAgentProxy commandAgent;

        /// <summary>
        /// Gets or sets the name and arguments of the command to execute.
        /// </summary>
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="OrchardCommand"/> object which represents the orchard command to execute.
        /// </summary>
        [Parameter(ParameterSetName = "CommandObject", ValueFromPipeline = true)]
        public OrchardCommand Command { get; set; }

        /// <summary>
        /// Gets or sets the switches which will be passed to the executed command.
        /// </summary>
        [Parameter(ParameterSetName = "Default", Position = 2, ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "CommandObject", Position = 2, ValueFromRemainingArguments = true)]
        public ArrayList Parameters { get; set; }

        /// <summary>
        /// Provides a one-time, preprocessing functionality for the cmdlet.
        /// </summary>
        protected override void BeginProcessing() 
        {
            base.BeginProcessing();
            this.commandAgent = this.AgentManager.GetAgent<CommandAgentProxy>();
        }

        /// <summary>
        /// Provides a record-by-record processing functionality for the cmdlet. 
        /// </summary>
        protected override void ProcessRecord() 
        {
            OrchardSite site = this.GetCurrentSite();
            string siteName = site != null ? site.Name : "Default";

            switch (this.ParameterSetName) 
            {
                case "Default":
                    this.InvokeDefault(siteName, this.Name, this.Parameters);
                    break;
                case "CommandObject":
                    this.InvokeCommandObject(siteName, this.Command, this.Parameters);
                    break;
            }
        }

        /// <summary>
        /// Invokes the Orchard command using the 'Default' parameters set.
        /// </summary>
        /// <param name="siteName">The name of the Orchard site on which the command will be execited.</param>
        /// <param name="commandName">The name and arguments of the command to execute.</param>
        /// <param name="parameters">The switches which will be passed to the executed command.</param>
        private void InvokeDefault(string siteName, string commandName, ArrayList parameters) 
        {
            var arguments = new List<string>();
            var switches = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(commandName)) 
            {
                arguments.Add(commandName);
            }

            this.InvokeWithParameters(siteName, arguments, switches, parameters);
        }

        /// <summary>
        /// Invokes the Orchard command using the 'CommandObject' parameters set.
        /// </summary>
        /// <param name="siteName">The name of the Orchard site on which the command will be execited.</param>
        /// <param name="command">
        /// The <see cref="OrchardCommand"/> object which represents the orchard command to execute.
        /// </param>
        /// <param name="parameters">The switches which will be passed to the executed command.</param>
        private void InvokeCommandObject(string siteName, OrchardCommand command, ArrayList parameters) 
        {
            var arguments = new List<string>(command.CommandName.Split(new[] { ' ' }));
            var switches = new Dictionary<string, string>();
            this.InvokeWithParameters(siteName, arguments, switches, parameters);
        }

        /// <summary>
        /// Invokes a legacy Orchard command.
        /// </summary>
        /// <param name="siteName">The name of the Orchard site on which the command will be execited.</param>
        /// <param name="arguments">The name and arguments of the command to execute.</param>
        /// <param name="switches">The switches which will be passed to the executed command.</param>
        /// <param name="parameters">The command parameters to parse.</param>
        private void InvokeWithParameters(
            string siteName, 
            List<string> arguments, 
            Dictionary<string, string> switches, 
            ArrayList parameters) 
        {
            if (parameters != null) 
            {
                IList<string> parsedArgs;
                IDictionary<string, string> parsedSwitches;
                if (this.ParseParameters(this.Parameters.ToArray().Cast<string>(), out parsedArgs, out parsedSwitches)) 
                {
                    arguments.AddRange(parsedArgs);
                    foreach (KeyValuePair<string, string> sw in parsedSwitches) 
                    {
                        switches.Add(sw.Key, sw.Value);
                    }
                }
                else 
                {
                    return;
                }
            }

            if (this.ShouldProcess("Site: " + siteName, "Invoke Command '" + string.Join(" ", arguments) + "'")) 
            {
                this.commandAgent.ExecuteCommand(
                    siteName,
                    arguments.ToArray(),
                    new Dictionary<string, string>(switches));
            }
        }

        /// <summary>
        /// Parses the specified command arguments and switches.
        /// </summary>
        /// <param name="args">The command arguments and switches to parse.</param>
        /// <param name="arguments">Parsed arguments.</param>
        /// <param name="switches">Parsed switches.</param>
        /// <returns>
        /// <c>true</c> if all arguments and switches were parsed successfully or <c>false</c> if an error ocurred.
        /// </returns>
        private bool ParseParameters(
            IEnumerable<string> args, 
            out IList<string> arguments, 
            out IDictionary<string, string> switches) 
        {
            arguments = new List<string>();
            switches = new Dictionary<string, string>();

            foreach (var arg in args) 
            {
                if (arg[0] == '/') 
                {
                    int index = arg.IndexOf(':');
                    string switchName = index < 0 ? arg.Substring(1) : arg.Substring(1, index - 1);
                    string switchValue = index < 0 || index >= arg.Length ? string.Empty : arg.Substring(index + 1);

                    if (string.IsNullOrEmpty(switchName)) 
                    {
                        string message = string.Format(
                            "Invalid switch syntax: \"{0}\". Valid syntax is /<switchName>[:<switchValue>].", 
                            arg);
                        var exception = new ArgumentException(message);
                        this.WriteError(exception, "InvalidCommandSwitchSyntax", ErrorCategory.SyntaxError);
                        return false;
                    }

                    switches.Add(switchName, switchValue);
                }
                else 
                {
                    arguments.Add(arg);
                }
            }

            return true;
        }
    }
}