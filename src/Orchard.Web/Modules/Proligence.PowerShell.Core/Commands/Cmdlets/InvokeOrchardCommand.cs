﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Orchard.Commands;
using Proligence.PowerShell.Core.Commands.Items;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Utilities;

namespace Proligence.PowerShell.Core.Commands.Cmdlets {
    [CmdletAlias("ioc")]
    [Cmdlet(VerbsLifecycle.Invoke, "OrchardCommand", DefaultParameterSetName = "Default", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Medium)]
    public class InvokeOrchardCommand : OrchardCmdlet {
        [Alias("n")]
        [ValidateNotNullOrEmpty]
        [Parameter(ParameterSetName = "Default", Mandatory = true, Position = 1)]
        public string Name { get; set; }

        [Parameter(ParameterSetName = "CommandObject", ValueFromPipeline = true)]
        public OrchardCommand Command { get; set; }

        [Parameter(ParameterSetName = "Default", Position = 2, ValueFromRemainingArguments = true)]
        [Parameter(ParameterSetName = "CommandObject", Position = 2, ValueFromRemainingArguments = true)]
        public ArrayList Parameters { get; set; }

        protected override void ProcessRecord() {
            string tenantName = this.GetCurrentTenantName() ?? "Default";
            string output = null;
            switch (ParameterSetName) {
                case "Default":
                    output = InvokeDefault(tenantName, Name, Parameters);
                    break;
                case "CommandObject":
                    output = InvokeCommandObject(tenantName, Command, Parameters);
                    break;
            }

            WriteObject(output);
        }

        private string InvokeDefault(string tenantName, string commandName, ArrayList parameters) {
            var arguments = new List<string>();
            var switches = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(commandName)) {
                arguments.Add(commandName);
            }

            return InvokeWithParameters(tenantName, arguments, switches, parameters);
        }

        private string InvokeCommandObject(string tenantName, OrchardCommand command, ArrayList parameters) {
            var arguments = new List<string>(command.CommandName.Split(' '));
            var switches = new Dictionary<string, string>();
            return InvokeWithParameters(tenantName, arguments, switches, parameters);
        }

        private string InvokeWithParameters(
            string tenantName,
            List<string> arguments,
            Dictionary<string, string> switches,
            ArrayList parameters) {
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
                    return null;
                }
            }

            if (ShouldProcess("Tenant: " + tenantName, "Invoke Command '" + string.Join(" ", arguments) + "'")) {
                return ExecuteCommand(
                    tenantName,
                    arguments.ToArray(),
                    new Dictionary<string, string>(switches));
            }

            return null;
        }

        /// <summary>
        /// Parses the specified command arguments and switches.
        /// </summary>
        /// <param name="args">The command arguments and switches to parse.</param>
        /// <param name="arguments">Parsed arguments.</param>
        /// <param name="switches">Parsed switches.</param>
        /// <returns>
        /// <c>true</c> if all arguments and switches were parsed successfully or <c>false</c> if an error occurred.
        /// </returns>
        private bool ParseParameters(
            IEnumerable<string> args,
            out IList<string> arguments,
            out IDictionary<string, string> switches) {
            arguments = new List<string>();
            switches = new Dictionary<string, string>();

            foreach (var arg in args) {
                if (arg[0] == '/') {
                    int index = arg.IndexOf(':');
                    string switchName = index < 0
                        ? arg.Substring(1)
                        : arg.Substring(1, index - 1);
                    string switchValue = index < 0 || index >= arg.Length
                        ? string.Empty
                        : arg.Substring(index + 1);

                    if (string.IsNullOrEmpty(switchName)) {
                        string message = string.Format(
                            CultureInfo.CurrentCulture,
                            "Invalid switch syntax: \"{0}\". Valid syntax is /<switchName>[:<switchValue>].",
                            arg);

                        WriteError(Error.Generic(
                            new ArgumentException(message),
                            "InvalidCommandSwitchSyntax",
                            ErrorCategory.SyntaxError));

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

        private string ExecuteCommand(
            string tenantName,
            IEnumerable<string> args,
            Dictionary<string, string> switches) {
            return this.UsingWorkContextScope(
                tenantName,
                scope => {
                    var commandManager = scope.Resolve<ICommandManager>();

                    using (TextWriter writer = new StringWriter(CultureInfo.CurrentCulture)) {
                        var parameters = new CommandParameters {
                            Arguments = args,
                            Switches = switches,
                            Input = TextReader.Null,
                            Output = writer
                        };

                        try {
                            commandManager.Execute(parameters);
                        }
                        catch (Exception ex) {
                            WriteError(Error.Generic(ex, "FailedToExecuteLegacyCommand"));
                            return null;
                        }

                        return writer.ToString();
                    }
                });
        }
    }
}