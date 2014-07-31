namespace Proligence.PowerShell.Provider.Console.UI {
    internal class OutputData {
        public string Output { get; set; }
        public OutputType Type { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }
        public dynamic Data { get; set; }
    }

    internal enum OutputType {
        Line,
        Warning,
        Error,
        Debug,
        Verbose,
        Progress
    }
}