using Proligence.PowerShell.Provider;
using Proligence.PowerShell.Provider.Console;

namespace OrchardPs.Console {
    public class CommandCompletionProvider {
        private readonly IPsSession _session;

        public CommandCompletionProvider(IPsSession session) {
            _session = session;
        }

        public CompletionData GetCommandCompletion(string command, int cursorPosition) {
            return _session.GetCommandCompletion(command, cursorPosition);
        }
    }
}