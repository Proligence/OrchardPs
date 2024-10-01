﻿using System;
using System.Collections.Generic;
using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Console.UI;

namespace OrchardPs.Console {
    public class DirectConsoleConnection : MarshalByRefObject, IConsoleConnection {
        private const ConsoleColor VerboseColor = ConsoleColor.Yellow;
        private const ConsoleColor WarningColor = ConsoleColor.Yellow;
        private const ConsoleColor DebugColor = ConsoleColor.Yellow;
        private const ConsoleColor ErrorColor = ConsoleColor.Red;
        private readonly IList<string> _inputHistory = new List<string>();
        private OutputData _lastOutputData;
        public CommandCompletionProvider CommandCompletionProvider { get; set; }

        public void Initialize() {
        }

        public override object InitializeLifetimeService() {
            return null;
        }

        public void Send(string connectionId, OutputData data) {
            if (data != null) {
                switch (data.Type) {
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

                _lastOutputData = data;
            }
        }

        public string GetInput() {
            string prompt = _lastOutputData != null
                ? _lastOutputData.Prompt
                : string.Empty;

            var consoleInputBuffer = new ConsoleInputBuffer(prompt, _inputHistory, CommandCompletionProvider);
            var line = consoleInputBuffer.ReadLine();
            _inputHistory.Insert(0, line);

            return line;
        }

        private static void WriteOutput(OutputData data) {
            string text = data.Output;

            if (data.NewLine) {
                text += Environment.NewLine;
            }

            ConsoleHelper.WriteToConsole(text, data.ForeColor);
        }

        private static void WriteOutput(OutputData data, ConsoleColor color) {
            string text = data.Output;

            if (data.NewLine) {
                text += Environment.NewLine;
            }

            ConsoleHelper.WriteToConsole(text, color);
        }
    }
}