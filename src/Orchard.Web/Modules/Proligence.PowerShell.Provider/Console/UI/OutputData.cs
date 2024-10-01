using System;

namespace Proligence.PowerShell.Provider.Console.UI {
    [Serializable]
    public class OutputData {
        /// <summary>
        /// The output text which should be written to the console.
        /// </summary>
        public string Output { get; set; }

        /// <summary>
        /// The current location console prompt (the text before the command entered by the user to the console).
        /// </summary>
        public string Prompt { get; set; }

        /// <summary>
        /// Specifies the type of the output.
        /// </summary>
        public OutputType Type { get; set; }

        /// <summary>
        /// The background (console) color.
        /// </summary>
        public string BackColor { get; set; }

        /// <summary>
        /// The foreground (text) color.
        /// </summary>
        public string ForeColor { get; set; }

        public bool Finished { get; set; }

        /// <summary>
        /// Determines whether a new line should be inserted after the output.
        /// </summary>
        public bool NewLine { get; set; }

        /// <summary>
        /// Additional output data.
        /// </summary>
        /// <remarks>
        /// Currently this is used only for reporting progress and autocompletion.
        /// </remarks>
        public dynamic Data { get; set; }
    }
}