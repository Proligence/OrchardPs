namespace Proligence.PowerShell.Provider.Console.UI {
    public class OutputData {
        public string Output { get; set; }
        public string Path { get; set; }
        public OutputType Type { get; set; }
        public string BackColor { get; set; }
        public string ForeColor { get; set; }
        public dynamic Data { get; set; }
    }

    public enum OutputType {
        Line,
        Warning,
        Error,
        Debug,
        Verbose,
        Progress
    }
}