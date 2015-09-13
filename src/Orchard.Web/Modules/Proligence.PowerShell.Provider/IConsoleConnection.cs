using Proligence.PowerShell.Provider.Console.UI;

namespace Proligence.PowerShell.Provider {
    /// <summary>
    /// Represents the connection between the PowerShell execution engine host and the console input/output UI.
    /// </summary>
    public interface IConsoleConnection {
        void Initialize();
        void Send(string connectionId, OutputData data);
    }
}