namespace Proligence.PowerShell.Provider
{
    using System.Collections;
    using System.Management.Automation.Runspaces;

    public sealed class PowerShellPipelineBuilder
    {
        private readonly Pipeline pipeline;

        public PowerShellPipelineBuilder(Pipeline pipeline)
        {
            this.pipeline = pipeline;
        }

        public PowerShellPipelineBuilder AddCommand(string commandText)
        {
            var command = new Command(commandText, false, false);
            this.pipeline.Commands.Add(command);
            return this;
        }

        public PowerShellPipelineBuilder AddScript(string scriptContents)
        {
            var command = new Command(scriptContents, true, false);
            this.pipeline.Commands.Add(command);
            return this;
        }

        public PowerShellPipelineBuilder AddInput(object obj)
        {
            this.pipeline.Input.Write(obj);
            return this;
        }

        public PowerShellPipelineBuilder AddInputCollection(IEnumerable collection)
        {
            this.pipeline.Input.Write(collection, true);
            return this;
        }

        public void Build()
        {
        }
    }
}