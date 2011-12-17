using System;
using System.Management.Automation;
using Orchard.Management.PsProvider.Vfs;

namespace Orchard.Management.PsProvider {
    public static class OrchardProviderExtensions {
        public static void WriteItemNode(this OrchardProvider provider, OrchardVfsNode node) {
            provider.WriteItemObject(node.Item, node.GetPath(), node is ContainerNode);
        }

        public static void WriteItemNode(this OrchardProvider provider, OrchardVfsNode node, Func<OrchardVfsNode, object> itemFunc) {
            provider.WriteItemObject(itemFunc(node), node.GetPath(), node is ContainerNode);
        }

        public static void WriteError(this OrchardProvider provider, Exception exception, string errorId, ErrorCategory category, object target = null) {
            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            provider.WriteError(errorRecord);
        }

        public static void ThrowTerminatingError(this OrchardProvider provider, Exception exception, string errorId, ErrorCategory category, object target = null) {
            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            provider.ThrowTerminatingError(errorRecord);
        }

        public static void Trace(this OrchardProvider provider, string format, params object[] args) {
            provider.WriteDebug("OrchardProvider::" + string.Format(format, args));
        }
    }
}