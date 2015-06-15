namespace Proligence.PowerShell.Provider
{
    using System;
    using System.Management.Automation;
    using System.Runtime.Serialization;

    [Serializable]
    public class PowerShellException : Exception
    {
        private readonly string command;
        private readonly ErrorRecord errorRecord;

        public string Command
        {
            get { return this.command; }
        }

        public ErrorRecord ErrorRecord
        {
            get { return this.errorRecord; }
        }

        public PowerShellException()
        {
        }
    
        public PowerShellException(string message)
            : base(message)
        {
        }

        public PowerShellException(string message, string command)
            : base(message)
        {
            this.command = command;
        }

        public PowerShellException(ErrorRecord errorRecord)
            : base(errorRecord.Exception.Message, errorRecord.Exception)
        {
        }
    
        public PowerShellException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected PowerShellException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.command = info.GetString("command");
            this.errorRecord = (ErrorRecord)info.GetValue("errorRecord", typeof(ErrorRecord));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("command", this.command);
            info.AddValue("errorRecord", this.errorRecord);
        }
    }
}