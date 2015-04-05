namespace Proligence.PowerShell.Provider
{
    using System;

    /// <summary>
    /// Specifies that the Orchard VFS node supports the specified cmdlets.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class SupportedCmdletAttribute : Attribute
    {
        private readonly string cmdletName;

        public SupportedCmdletAttribute(string cmdletName)
        {
            this.cmdletName = cmdletName;
        }

        /// <summary>
        /// Gets the name of the supported cmdlet.
        /// </summary>
        public string CmdletName 
        { 
            get { return this.cmdletName; }
        }
    }
}