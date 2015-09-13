using System;
using System.Management.Automation;
using System.Runtime.Serialization;

namespace Proligence.PowerShell.Provider {
    [Serializable]
    public class PowerShellException : Exception {
        private readonly string _command;
        private readonly ErrorRecord _errorRecord;

        public string Command {
            get { return _command; }
        }

        public ErrorRecord ErrorRecord {
            get { return _errorRecord; }
        }

        public PowerShellException() {
        }

        public PowerShellException(string message)
            : base(message) {
        }

        public PowerShellException(string message, string command)
            : base(message) {
            _command = command;
        }

        public PowerShellException(ErrorRecord errorRecord)
            : base(errorRecord.Exception.Message, errorRecord.Exception) {
        }

        public PowerShellException(string message, Exception innerException)
            : base(message, innerException) {
        }

        protected PowerShellException(SerializationInfo info, StreamingContext context)
            : base(info, context) {
            _command = info.GetString("command");
            _errorRecord = (ErrorRecord) info.GetValue("errorRecord", typeof (ErrorRecord));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);

            info.AddValue("command", _command);
            info.AddValue("errorRecord", _errorRecord);
        }
    }
}