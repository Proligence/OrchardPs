namespace OrchardPs.Console
{
    using System;
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Console.UI;

    public class DirectConsoleConnection : MarshalByRefObject, IConsoleConnection
    {
        private const ConsoleColor VerboseColor = ConsoleColor.Yellow;
        private const ConsoleColor WarningColor = ConsoleColor.Yellow;
        private const ConsoleColor DebugColor = ConsoleColor.Yellow;
        private const ConsoleColor ErrorColor = ConsoleColor.Red;

        private OutputData lastOutputData;

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
                switch (data.Type)
                {
                    case OutputType.Verbose:
                        data.Output = "VERBOSE: " + data.Output;
                        WriteOutput(data, VerboseColor);
                        break;
                    
                    case OutputType.Warning:
                        data.Output = "WARNING: " + data.Output;
                        WriteOutput(data, WarningColor);
                        break;

                    case OutputType.Debug:
                        data.Output = "DEBUG: " + data.Output;
                        WriteOutput(data, DebugColor);
                        break;

                    case OutputType.Error:
                        WriteOutput(data, ErrorColor);
                        break;

                    ////TODO:
                    ////case OutputType.Line:
                    ////case OutputType.Progress:
                    ////case OutputType.Completion:

                    default:
                        WriteOutput(data);
                        break;
                }

                this.lastOutputData = data;
            }
        }

        public string GetInput()
        {
            string prompt = this.lastOutputData != null
                ? this.lastOutputData.Prompt
                : string.Empty;

            return new ConsoleInputBuffer(prompt).ReadLine();
        }

        private static void WriteOutput(OutputData data)
        {
            string text = data.Output;

            if (data.NewLine)
            {
                text += Environment.NewLine;
            }

            ConsoleHelper.WriteToConsole(text, data.ForeColor);
        }

        private static void WriteOutput(OutputData data, ConsoleColor color)
        {
            string text = data.Output;

            if (data.NewLine)
            {
                text += Environment.NewLine;
            }

            ConsoleHelper.WriteToConsole(text, color);
        }
    }
}