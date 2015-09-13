using System;

namespace Proligence.PowerShell.Provider {
    /// <summary>
    /// Specifies that the Orchard VFS node supports the specified cmdlets.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class SupportedCmdletAttribute : Attribute {
        private readonly string _cmdletName;

        public SupportedCmdletAttribute(string cmdletName) {
            _cmdletName = cmdletName;
        }

        /// <summary>
        /// Gets the name of the supported cmdlet.
        /// </summary>
        public string CmdletName {
            get { return _cmdletName; }
        }
    }
}