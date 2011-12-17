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

        public static void WriteError(
            this OrchardProvider provider, Exception exception, string errorId, ErrorCategory category, object target = null) {

            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            provider.WriteError(errorRecord);
        }

        public static void ThrowTerminatingError(
            this OrchardProvider provider, Exception exception, string errorId, ErrorCategory category, object target = null) {

            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            provider.ThrowTerminatingError(errorRecord);
        }

        public static void Trace(this OrchardProvider provider, string format, params object[] args) {
            provider.WriteDebug("OrchardProvider::" + string.Format(format, args));
        }

        public static void Try(
            this OrchardProvider provider, Action action, string errorId, ErrorCategory category = ErrorCategory.NotSpecified, object target = null) {

            try {
                action();
            }
            catch (OrchardProviderException ex) {
                string err = errorId ?? ex.ErrorId;
                ErrorCategory cat = category != ErrorCategory.NotSpecified ? category : ex.ErrorCategory;

                if (ex.IsFatal) {
                    provider.ThrowTerminatingError(ex, err, cat, target);
                }
                else {
                    provider.WriteError(ex, err, cat, target);
                }
            }
            catch (Exception ex) {
                ErrorCategory cat = category != ErrorCategory.NotSpecified ? category : GetCategoryFromException(ex);
                provider.WriteError(ex, errorId, cat, target);
            }
        }

        public static void TryCritical(
            this OrchardProvider provider, Action action, string errorId, ErrorCategory category = ErrorCategory.NotSpecified, object target = null) {

            try {
                action();
            }
            catch (OrchardProviderException ex) {
                provider.ThrowTerminatingError(
                    ex,
                    errorId ?? ex.ErrorId,
                    category != ErrorCategory.NotSpecified ? category : ex.ErrorCategory,
                    target);
            }
            catch (Exception ex) {
                ErrorCategory cat = category != ErrorCategory.NotSpecified ? category : GetCategoryFromException(ex);
                provider.ThrowTerminatingError(ex, errorId, cat, target);
            }
        }

        private static ErrorCategory GetCategoryFromException(Exception ex) {
            var providerException = ex as OrchardProviderException;
            if (providerException != null) {
                return providerException.ErrorCategory;
            }

            Type exceptionType = ex.GetType();

            if (exceptionType == typeof(ArgumentException)) {
                return ErrorCategory.InvalidArgument;
            }

            if (exceptionType == typeof(InvalidOperationException)) {
                return ErrorCategory.InvalidOperation;
            }

            if (exceptionType == typeof(NotSupportedException)) {
                return ErrorCategory.NotImplemented;
            }

            return ErrorCategory.NotSpecified;
        }
    }
}