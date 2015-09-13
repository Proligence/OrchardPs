using System;
using System.Management.Automation;
using System.Security;

namespace Proligence.PowerShell.Provider.Utilities {
    /// <summary>
    /// Implements a facade which simplifies creating <see cref="ErrorRecord"/> objects.
    /// </summary>
    public static class Error {
        public static ErrorRecord Generic(
            Exception exception,
            string errorId,
            ErrorCategory category = ErrorCategory.NotSpecified,
            string target = null) {
            return new ErrorRecord(exception, errorId, category, target);
        }

        public static ErrorRecord InvalidArgument(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.InvalidArgument, target);
        }

        public static ErrorRecord InvalidArgument(string message, string errorId, string target = null) {
            var exception = new ArgumentException(message);
            return new ErrorRecord(exception, errorId, ErrorCategory.InvalidArgument, target);
        }

        public static ErrorRecord ArgumentNull(string errorId, string target = null) {
            var exception = new ArgumentNullException();
            return new ErrorRecord(exception, errorId, ErrorCategory.InvalidArgument, target);
        }

        public static ErrorRecord InvalidOperation(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.InvalidOperation, target);
        }

        public static ErrorRecord InvalidOperation(string message, string errorId, string target = null) {
            var exception = new InvalidOperationException(message);
            return new ErrorRecord(exception, errorId, ErrorCategory.InvalidOperation, target);
        }

        public static ErrorRecord InvalidData(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.InvalidData, target);
        }

        public static ErrorRecord InvalidType(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.InvalidType, target);
        }

        public static ErrorRecord InvalidResult(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.InvalidResult, target);
        }

        public static ErrorRecord ObjectNotFound(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.ObjectNotFound, target);
        }

        public static ErrorRecord SecurityError(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.SecurityError, target);
        }

        public static ErrorRecord SecurityError(string message, string errorId, string target = null) {
            var exception = new SecurityException(message);
            return new ErrorRecord(exception, errorId, ErrorCategory.SecurityError, target);
        }

        public static ErrorRecord PermissionDenied(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.PermissionDenied, target);
        }

        public static ErrorRecord PermissionDenied(string message, string errorId, string target = null) {
            var exception = new AccessViolationException(message);
            return new ErrorRecord(exception, errorId, ErrorCategory.PermissionDenied, target);
        }

        public static ErrorRecord SyntaxError(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.SyntaxError, target);
        }

        public static ErrorRecord NotImplemented(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.NotImplemented, target);
        }

        public static ErrorRecord NotImplemented(string message, string errorId, string target = null) {
            var exception = new NotImplementedException(message);
            return new ErrorRecord(exception, errorId, ErrorCategory.NotImplemented, target);
        }

        public static ErrorRecord NotSpecified(Exception exception, string errorId, string target = null) {
            return new ErrorRecord(exception, errorId, ErrorCategory.NotSpecified, target);
        }

        public static ErrorRecord FailedToFindTenant(string tenantName) {
            return InvalidArgument(
                "Failed to find tenant with name '" + tenantName + "'.",
                "FailedToFindTenant");
        }

        public static ErrorRecord CannotAlterDefaultTenant() {
            return InvalidOperation(
                "The operation cannot be performed on the default tenant.",
                "CannotAlterDefaultTenant");
        }
    }
}