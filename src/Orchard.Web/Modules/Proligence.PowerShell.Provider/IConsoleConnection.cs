namespace Proligence.PowerShell.Provider
{
    using Proligence.PowerShell.Provider.Console.UI;

    public interface IConsoleConnection
    {
        void Initialize();
        void Send(string connectionId, OutputData data);
    }
}