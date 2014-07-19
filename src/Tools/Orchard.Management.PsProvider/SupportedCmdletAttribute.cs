namespace Orchard.Management.PsProvider
{
    using System;

    /// <summary>
    /// Specifies that the Orchard VFS node supports the specified cmdlets.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class SupportedCmdletAttribute : Attribute
    {
        /// <summary>
        /// The cmdletName of the supported cmdlet.
        /// </summary>
        private readonly string cmdletName;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupportedCmdletAttribute"/> class.
        /// </summary>
        /// <param name="cmdletName">The cmdletName of the supported cmdlet.</param>
        public SupportedCmdletAttribute(string cmdletName)
        {
            this.cmdletName = cmdletName;
        }

        /// <summary>
        /// Gets the cmdletName of the supported cmdlet.
        /// </summary>
        public string CmdletName 
        { 
            get { return this.cmdletName; }
        }
    }
}