namespace Orchard.Tests.PowerShell.Infrastructure
{
    using System;
    using System.Text;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Console.UI;

    public class TestConsoleConnection : MarshalByRefObject, IConsoleConnection
    {
        public TestConsoleConnection()
        {
            this.VerboseOutput = new StringBuilder();
            this.WarningOutput = new StringBuilder();
            this.DebugOutput = new StringBuilder();
            this.ErrorOutput = new StringBuilder();
            this.Output = new StringBuilder();
        }

        public StringBuilder VerboseOutput { get; private set; }
        public StringBuilder WarningOutput { get; private set; }
        public StringBuilder DebugOutput { get; private set; }
        public StringBuilder ErrorOutput { get; private set; }
        public StringBuilder Output { get; private set; }

        public OutputData LastOutputData { get; private set; }

        public void Initialize()
        {
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Send(string connectionId, OutputData data)
        {
            if (data != null)
            {
                this.LastOutputData = data;

                if (data.Output != null)
                {
                    string output = data.NewLine 
                        ? data.Output + Environment.NewLine
                        : data.Output;

                    if (data.Type == OutputType.Verbose)
                    {
                        this.VerboseOutput.Append(output);
                    }
                    else if (data.Type == OutputType.Warning)
                    {
                        this.WarningOutput.Append(output);
                    }
                    else if (data.Type == OutputType.Debug)
                    {
                        this.DebugOutput.Append(output);
                    }
                    else if (data.Type == OutputType.Error)
                    {
                        this.ErrorOutput.Append(output);
                    }
                    else
                    {
                        this.Output.Append(output);                        
                    }
                }
            }
        }

        public void Reset()
        {
            this.VerboseOutput.Clear();
            this.WarningOutput.Clear();
            this.DebugOutput.Clear();
            this.ErrorOutput.Clear();
            this.Output.Clear();
        }
    }
}