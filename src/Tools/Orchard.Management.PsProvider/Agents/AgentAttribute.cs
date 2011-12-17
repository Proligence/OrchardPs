using System;

namespace Orchard.Management.PsProvider.Agents {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AgentAttribute : Attribute {
        public string TypeName { get; private set; }

        public AgentAttribute(string typeName) {
            TypeName = typeName;
        }
    }
}