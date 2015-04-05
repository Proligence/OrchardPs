namespace Proligence.PowerShell.Provider
{
    using System;

    /// <summary>
    /// Specifies an alias for a cmdlet which will be automatically registered during application startup.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class CmdletAliasAttribute : Attribute 
    {
        public CmdletAliasAttribute(string alias) 
        {
            if (string.IsNullOrEmpty(alias)) 
            {
                throw new ArgumentNullException("alias");
            }
            
            this.Alias = alias;
        }

        public string Alias { get; private set; }
    }
}