namespace Proligence.PowerShell.Provider
{
    using Proligence.PowerShell.Provider.Console.UI;

    /// <summary>
    /// Represents the connection between the PowerShell execution engine host and the console input/output UI.
    /// </summary>
    public interface IConsoleConnection
    {
        void Initialize();
        void Send(string connectionId, OutputData data);
    }
}