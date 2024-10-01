using System.Collections;
using System.Management.Automation.Runspaces;

namespace Proligence.PowerShell.Provider {
    public sealed class PowerShellPipelineBuilder {
        private readonly Pipeline _pipeline;

        public PowerShellPipelineBuilder(Pipeline pipeline) {
            _pipeline = pipeline;
        }

        public PowerShellPipelineBuilder AddCommand(string commandText) {
            var command = new Command(commandText, false, false);
            _pipeline.Commands.Add(command);
            return this;
        }

        public PowerShellPipelineBuilder AddScript(string scriptContents) {
            var command = new Command(scriptContents, true, false);
            _pipeline.Commands.Add(command);
            return this;
        }

        public PowerShellPipelineBuilder AddInput(object obj) {
            _pipeline.Input.Write(obj);
            return this;
        }

        public PowerShellPipelineBuilder AddInputCollection(IEnumerable collection) {
            _pipeline.Input.Write(collection, true);
            return this;
        }

        public void Build() {
        }
    }
}