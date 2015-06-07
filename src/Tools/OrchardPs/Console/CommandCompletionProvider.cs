namespace OrchardPs.Console
{
    using Proligence.PowerShell.Provider;
    using Proligence.PowerShell.Provider.Console;

    public class CommandCompletionProvider
    {
        private readonly IPsSession session;

        public CommandCompletionProvider(IPsSession session)
        {
            this.session = session;
        }

        public CompletionData GetCommandCompletion(string command, int cursorPosition)
        {
            return this.session.GetCommandCompletion(command, cursorPosition);
        }
    }
}