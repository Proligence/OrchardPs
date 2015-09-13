using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Proligence.PowerShell.Provider.Console;

namespace OrchardPs.Console {
    internal class ConsoleInputBuffer {
        private readonly string _prompt;
        private readonly IList<string> _inputHistory;
        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly CommandCompletionProvider _commandCompletionProvider;
        private int? _historyPosition;
        private string _completionCommand;
        private CompletionData _completionResult;
        private int _completionBufferIndex;
        private int? _completionMatchIndex;

        public ConsoleInputBuffer(
            string prompt,
            IList<string> inputHistory,
            CommandCompletionProvider commandCompletionProvider = null) {
            _prompt = prompt ?? string.Empty;
            _inputHistory = inputHistory ?? new List<string>();
            _commandCompletionProvider = commandCompletionProvider;

            System.Console.Write(prompt);
        }

        public string ReadLine() {
            var key = System.Console.ReadKey(true);
            while (true) {
                switch (key.Key) {
                    case ConsoleKey.Enter:
                        System.Console.WriteLine();
                        return _buffer.ToString();
                    case ConsoleKey.LeftArrow:
                        HandleLeftArrow(key.Modifiers.HasFlag(ConsoleModifiers.Control));
                        break;
                    case ConsoleKey.RightArrow:
                        HandleRightArrow(key.Modifiers.HasFlag(ConsoleModifiers.Control));
                        break;
                    case ConsoleKey.UpArrow:
                        HandleUpArrow();
                        break;
                    case ConsoleKey.DownArrow:
                        HandleDownArrow();
                        break;
                    case ConsoleKey.Home:
                        HandleHome();
                        break;
                    case ConsoleKey.End:
                        HandleEnd();
                        break;
                    case ConsoleKey.Backspace:
                        HandleBackspace();
                        break;
                    case ConsoleKey.Delete:
                        HandleDelete();
                        break;
                    case ConsoleKey.Escape:
                        HandleEscape();
                        break;
                    case ConsoleKey.Tab:
                        HandleTab(key.Modifiers.HasFlag(ConsoleModifiers.Shift));
                        break;

                    // NOTE: 255 is returned before Home and End, we need to discard it
                    case (ConsoleKey) 255:
                        break;

                    default:
                        InsertCharIntoBuffer(key.KeyChar);
                        break;
                }

                key = System.Console.ReadKey(true);
            }
        }

        private void HandleLeftArrow(bool ctrl) {
            if (ctrl) {
                int index = System.Console.CursorLeft - _prompt.Length - 1;
                if (index > 0) {
                    // Skip any whitespace
                    while ((index > 0) && char.IsWhiteSpace(_buffer[index])) {
                        index--;
                    }

                    // Move to the beginning of the current word
                    if (!char.IsWhiteSpace(_buffer[index]) && !char.IsWhiteSpace(_buffer[index - 1])) {
                        while ((index > 0) && !char.IsWhiteSpace(_buffer[index - 1])) {
                            index--;
                        }
                    }

                    System.Console.CursorLeft = _prompt.Length + index;
                }
            }
            else {
                if (System.Console.CursorLeft > _prompt.Length) {
                    System.Console.CursorLeft--;
                }
            }
        }

        private void HandleRightArrow(bool ctrl) {
            if (ctrl) {
                int index = System.Console.CursorLeft - _prompt.Length;
                if (index < _buffer.Length) {
                    // Move to the end of the current word
                    while ((index < _buffer.Length) && !char.IsWhiteSpace(_buffer[index])) {
                        index++;
                    }

                    // Skip any whitespace
                    while ((index < _buffer.Length) && char.IsWhiteSpace(_buffer[index])) {
                        index++;
                    }

                    System.Console.CursorLeft = _prompt.Length + index;
                }
            }
            else {
                if (System.Console.CursorLeft < (_prompt.Length + _buffer.Length)) {
                    System.Console.CursorLeft++;
                }
            }
        }

        private void HandleUpArrow() {
            if (_historyPosition == null) {
                _historyPosition = 0;
                RestoreHistoryInput();
            }
            else if (_historyPosition < _inputHistory.Count - 1) {
                _historyPosition++;
                RestoreHistoryInput();
            }
        }

        private void HandleDownArrow() {
            if ((_historyPosition != null) && (_historyPosition.Value > 0)) {
                _historyPosition--;
                RestoreHistoryInput();
            }
        }

        private void HandleHome() {
            System.Console.CursorLeft = _prompt.Length;
        }

        private void HandleEnd() {
            System.Console.CursorLeft = _prompt.Length + _buffer.Length;
        }

        private void RestoreHistoryInput() {
            if (_historyPosition != null) {
                int index = _historyPosition.Value;
                if ((index >= 0) && (index < _inputHistory.Count)) {
                    System.Console.Write("\r" + _prompt + new string(' ', _buffer.Length));
                    _buffer.Clear();
                    _buffer.Append(_inputHistory[_historyPosition.Value]);
                    System.Console.Write("\r" + _prompt + _buffer);

                    // Discard command completion data.
                    _completionCommand = null;
                }
            }
        }

        private void HandleBackspace() {
            if (System.Console.CursorLeft > _prompt.Length) {
                System.Console.Write("\b");

                int index = System.Console.CursorLeft;
                int bufferIndex = index - _prompt.Length;
                if (_buffer.Length > bufferIndex) {
                    _buffer.Remove(bufferIndex, 1);
                    System.Console.Write(_buffer.ToString().Substring(bufferIndex) + " ");
                    System.Console.CursorLeft = index;
                }
                else {
                    System.Console.Write(" \b");
                }

                // Discard command completion data.
                _completionCommand = null;
            }
        }

        private void HandleDelete() {
            if (System.Console.CursorLeft < _prompt.Length + _buffer.Length) {
                int index = System.Console.CursorLeft;
                int bufferIndex = index - _prompt.Length;
                string tail = _buffer.ToString().Substring(bufferIndex + 1);
                _buffer.Remove(bufferIndex, 1);
                System.Console.Write(tail + " ");
                System.Console.CursorLeft = index;

                // Discard command completion data.
                _completionCommand = null;
            }
        }

        private void HandleEscape() {
            System.Console.Write("\r" + _prompt + new string(' ', _buffer.Length));
            System.Console.CursorLeft = _prompt.Length;
            _buffer.Clear();

            // Discard command completion data.
            _completionCommand = null;
        }

        private void HandleTab(bool shift) {
            if (_commandCompletionProvider != null) {
                if (_completionCommand == null) {
                    LoadCommandCompletion();
                }

                if (_completionMatchIndex == null) {
                    if (_completionResult.Results.Any()) {
                        _completionMatchIndex = 0;
                    }
                }
                else {
                    if (shift) {
                        if (_completionMatchIndex.Value == 0) {
                            _completionMatchIndex = _completionResult.Results.Count - 1;
                        }
                        else {
                            _completionMatchIndex--;
                        }
                    }
                    else {
                        _completionMatchIndex++;
                        _completionMatchIndex %= _completionResult.Results.Count;
                    }
                }

                InsertCommandCompletion();
            }
        }

        private void LoadCommandCompletion() {
            _completionCommand = _buffer.ToString();
            _completionBufferIndex = System.Console.CursorLeft - _prompt.Length;
            _completionMatchIndex = null;

            _completionResult = _commandCompletionProvider.GetCommandCompletion(
                _completionCommand,
                _completionBufferIndex);
        }

        private void InsertCommandCompletion() {
            if (_completionMatchIndex != null) {
                int index = _completionMatchIndex.Value;
                if ((index >= 0) && (index < _completionResult.Results.Count)) {
                    // Clear buffer line
                    System.Console.Write("\r" + _prompt + new string(' ', _buffer.Length));
                    System.Console.CursorLeft = _prompt.Length;
                    _buffer.Clear();

                    _buffer.Append(_completionCommand);
                    _buffer.Remove(_completionResult.ReplacementIndex, _completionResult.ReplacementLength);
                    _buffer.Insert(_completionResult.ReplacementIndex, _completionResult.Results[index]);
                    System.Console.Write("\r" + _prompt + _buffer);

                    System.Console.CursorLeft =
                        _prompt.Length +
                        _completionResult.ReplacementIndex +
                        _completionResult.Results[index].Length;
                }
            }
        }

        private void InsertCharIntoBuffer(char chr) {
            if (System.Console.CursorLeft == (_prompt.Length + _buffer.Length)) {
                _buffer.Append(chr);
                System.Console.Write(chr);
            }
            else {
                int index = System.Console.CursorLeft - _prompt.Length;
                string tail = _buffer.ToString().Substring(index);
                _buffer.Insert(index, chr);
                System.Console.Write(chr);
                System.Console.Write(tail);
                System.Console.CursorLeft = _prompt.Length + index + 1;
            }

            // Discard command completion data.
            _completionCommand = null;
        }
    }
}