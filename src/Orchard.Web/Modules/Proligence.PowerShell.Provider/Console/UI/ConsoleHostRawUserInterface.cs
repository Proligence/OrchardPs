using System;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace Proligence.PowerShell.Provider.Console.UI {
    public class ConsoleHostRawUserInterface : PSHostRawUserInterface {
        // ReSharper disable once NotAccessedField.Local
        private readonly ConsoleHostUserInterface _consoleHostUserInterface;

        public ConsoleHostRawUserInterface(ConsoleHostUserInterface consoleHostUserInterface) {
            _consoleHostUserInterface = consoleHostUserInterface;

            /* ReSharper disable DoNotCallOverridableMethodsInConstructor */
            ForegroundColor = ConsoleColor.White;
            BackgroundColor = ConsoleColor.Blue;
            BufferSize = new Size(200, 1000);
            WindowSize = new Size(200, 72);
            /* ReSharper restore DoNotCallOverridableMethodsInConstructor */
        }

        public override ConsoleColor ForegroundColor { get; set; }
        public override ConsoleColor BackgroundColor { get; set; }
        public override Coordinates CursorPosition { get; set; }
        public override Coordinates WindowPosition { get; set; }
        public override int CursorSize { get; set; }
        public override Size BufferSize { get; set; }
        public override Size WindowSize { get; set; }

        public override Size MaxWindowSize {
            get {
                ThrowNotInteractive();
                return default(Size);
            }
        }

        public override Size MaxPhysicalWindowSize {
            get {
                ThrowNotInteractive();
                return default(Size);
            }
        }

        public override bool KeyAvailable {
            get {
                ThrowNotInteractive();
                return false;
            }
        }

        public override string WindowTitle { get; set; }

        public override KeyInfo ReadKey(ReadKeyOptions options) {
            ThrowNotInteractive();
            return default(KeyInfo);
        }

        public override void FlushInputBuffer() {
            ThrowNotInteractive();
        }

        public override void SetBufferContents(Coordinates origin, BufferCell[,] contents) {
            ThrowNotInteractive();
        }

        public override void SetBufferContents(Rectangle rectangle, BufferCell fill) {
            ThrowNotInteractive();
        }

        public override BufferCell[,] GetBufferContents(Rectangle rectangle) {
            ThrowNotInteractive();
            return null;
        }

        public override void ScrollBufferContents(Rectangle source, Coordinates destination, Rectangle clip,
            BufferCell fill) {
            ThrowNotInteractive();
        }

        internal void ThrowNotInteractive() {
            string message =
                "A command that prompts the user failed because the host program or the command type does not support" +
                "user interaction. Try a host program that supports user interaction, such as the Windows PowerShell " +
                "Console or Windows PowerShell ISE, and remove prompt-related commands from command types that do not " +
                "support user interaction, such as Windows PowerShell workflows.";

            throw new HostException(message, null, "HostFunctionNotImplemented", ErrorCategory.NotImplemented);
        }
    }
}