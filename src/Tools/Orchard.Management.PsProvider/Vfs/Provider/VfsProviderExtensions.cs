namespace Orchard.Management.PsProvider.Vfs.Provider
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Management.Automation;
    using Orchard.Management.PsProvider.Vfs.Navigation;

    /// <summary>
    /// Implements extension methods for the <see cref="VfsProvider"/> class.
    /// </summary>
    public static class VfsProviderExtensions
    {
        /// <summary>
        /// Writes the specified <see cref="VfsNode"/> to the provider's output.
        /// </summary>
        /// <param name="provider">The PowerShell VFS PS provider instance.</param>
        /// <param name="node">The node instance.</param>
        public static void WriteItemNode(this VfsProvider provider, VfsNode node) 
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            provider.WriteItemObject(node.Item ?? node.Name, node.GetPath(), node is ContainerNode);
        }

        /// <summary>
        /// Writes the specified <see cref="VfsNode"/> to the provider's output.
        /// </summary>
        /// <param name="provider">The PowerShell VFS PS provider instance.</param>
        /// <param name="node">The node instance.</param>
        /// <param name="itemFunc">The function which calculates the object to output based on the node.</param>
        public static void WriteItemNode(
            this VfsProvider provider, 
            VfsNode node, 
            Func<VfsNode, object> itemFunc) 
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            if (itemFunc == null)
            {
                throw new ArgumentNullException("itemFunc");
            }

            provider.WriteItemObject(itemFunc(node), node.GetPath(), node is ContainerNode);
        }

        /// <summary>
        /// Writes a non-terminating error message to the PS provider.
        /// </summary>
        /// <param name="provider">The PowerShell VFS PS provider instance.</param>
        /// <param name="exception">The exception which contains the details of the error.</param>
        /// <param name="errorId">The error identifier (for PowerShell).</param>
        /// <param name="category">The error category (for PowerShell).</param>
        /// <param name="target">The target object of the current operation (for PowerShell), optional.</param>
        public static void WriteError(
            this VfsProvider provider, 
            Exception exception, 
            string errorId, 
            ErrorCategory category, 
            object target = null) 
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            provider.WriteError(errorRecord);
        }

        /// <summary>
        /// Writes a terminating error message to the PS provider.
        /// </summary>
        /// <param name="provider">The PowerShell VFS PS provider instance.</param>
        /// <param name="exception">The exception which contains the details of the error.</param>
        /// <param name="errorId">The error identifier (for PowerShell).</param>
        /// <param name="category">The error category (for PowerShell).</param>
        /// <param name="target">The target object of the current operation (for PowerShell), optional.</param>
        public static void ThrowTerminatingError(
            this VfsProvider provider, 
            Exception exception, 
            string errorId, 
            ErrorCategory category, 
            object target = null)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            var errorRecord = new ErrorRecord(exception, errorId, category, target);
            provider.ThrowTerminatingError(errorRecord);
        }

        /// <summary>
        /// Writes the specified diagnostic message.
        /// </summary>
        /// <param name="provider">The PowerShell VFS PS provider instance.</param>
        /// <param name="format">The message's format string.</param>
        /// <param name="args">The arguments for the message's format string.</param>
        public static void Trace(this VfsProvider provider, string format, params object[] args) 
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            provider.WriteDebug("VfsProvider::" + string.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Executes the specified action and performs error checking.
        /// </summary>
        /// <param name="provider">The PowerShell VFS PS provider instance.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="errorId">The error identifier in case an error occurs (for PowerShell).</param>
        /// <param name="category">The error category (for PowerShell) in case an error occurs.</param>
        /// <param name="target">The target object of the current operation (for PowerShell), optional.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design")]
        public static void Try(
            this VfsProvider provider, 
            Action action, 
            string errorId, 
            ErrorCategory category = ErrorCategory.NotSpecified, 
            object target = null) 
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            try 
            {
                action();
            }
            catch (VfsProviderException ex) 
            {
                string err = errorId ?? ex.ErrorId;
                ErrorCategory cat = category != ErrorCategory.NotSpecified ? category : ex.ErrorCategory;

                if (ex.IsFatal) 
                {
                    provider.ThrowTerminatingError(ex, err, cat, target);
                }
                else 
                {
                    provider.WriteError(ex, err, cat, target);
                }
            }
            catch (Exception ex) 
            {
                ErrorCategory cat = category != ErrorCategory.NotSpecified ? category : GetCategoryFromException(ex);
                provider.WriteError(ex, errorId, cat, target);
            }
        }

        /// <summary>
        /// Executes the specified action and performs error checking. If an error occurs, the current pipeline is 
        /// terminated.
        /// </summary>
        /// <param name="provider">The PowerShell VFS PS provider instance.</param>
        /// <param name="action">The action to invoke.</param>
        /// <param name="errorId">The error identifier in case an error occurs (for PowerShell).</param>
        /// <param name="category">The error category (for PowerShell) in case an error occurs.</param>
        /// <param name="target">The target object of the current operation (for PowerShell), optional.</param>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "By design")]
        public static void TryCritical(
            this VfsProvider provider, 
            Action action, 
            string errorId, 
            ErrorCategory category = ErrorCategory.NotSpecified, 
            object target = null) 
        {
            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            try 
            {
                action();
            }
            catch (VfsProviderException ex) 
            {
                provider.ThrowTerminatingError(
                    ex,
                    errorId ?? ex.ErrorId,
                    category != ErrorCategory.NotSpecified ? category : ex.ErrorCategory,
                    target);
            }
            catch (Exception ex) 
            {
                ErrorCategory cat = category != ErrorCategory.NotSpecified ? category : GetCategoryFromException(ex);
                provider.ThrowTerminatingError(ex, errorId, cat, target);
            }
        }

        /// <summary>
        /// Gets the <see cref="ErrorCategory"/> value based on the exception object.
        /// </summary>
        /// <param name="ex">The exception object.</param>
        /// <returns>The <see cref="ErrorCategory"/> value.</returns>
        private static ErrorCategory GetCategoryFromException(Exception ex) 
        {
            var providerException = ex as VfsProviderException;
            if (providerException != null) 
            {
                return providerException.ErrorCategory;
            }

            Type exceptionType = ex.GetType();

            if (exceptionType == typeof(ArgumentException)) 
            {
                return ErrorCategory.InvalidArgument;
            }

            if (exceptionType == typeof(InvalidOperationException)) 
            {
                return ErrorCategory.InvalidOperation;
            }

            if (exceptionType == typeof(NotSupportedException)) 
            {
                return ErrorCategory.NotImplemented;
            }

            return ErrorCategory.NotSpecified;
        }
    }
}