namespace Proligence.PowerShell.Provider.Models
{
    using System;

    /// <remarks>
    /// We need these settings in a separate class which is serializable, so that it can be passed between AppDomain
    /// boundries, so that OrchardPs.exe can use it.
    /// </remarks>
    [Serializable]
    public class PowerShellSettings : IPowerShellSettings
    {
        public const int DefaultConsoleWidth = 120;
        public const int MinimumConsoleWidth = 40;
        public const int MaximumConsoleWidth = 400;
        public const int DefaultConsoleHeight = 50;
        public const int MinimumConsoleHeight = 40;
        public const int MaximumConsoleHeight = 400;

        public int ConsoleWidth { get; set; }
        public int ConsoleHeight { get; set; }
    }
}