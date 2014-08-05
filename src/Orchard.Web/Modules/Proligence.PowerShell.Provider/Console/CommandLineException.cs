namespace Proligence.PowerShell.Provider.Console 
{
    using System;
    using System.Globalization;

    public class CommandLineException : Exception
    {
        public CommandLineException(string executablePath, string arguments, string message)
            : base(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2} {3}", message, Environment.NewLine, executablePath, arguments))
        {
        }

        public int ExitCode { get; set; }
        public string Output { get; set; }
        public string Error { get; set; }

        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "ExitCode: {0}, Output: {1}, Error: {2}, {3}", 
                ExitCode,
                Output,
                Error,
                base.ToString());
        }
    }
}