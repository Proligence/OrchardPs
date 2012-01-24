using System;

namespace Orchard.Management.PsProvider {
    public class CmdletAliasAttribute : Attribute {
        public CmdletAliasAttribute(string alias) {
            if (string.IsNullOrEmpty(alias)) {
                throw new ArgumentNullException("alias");
            }
            
            Alias = alias;
        }

        public string Alias { get; private set; }
    }
}