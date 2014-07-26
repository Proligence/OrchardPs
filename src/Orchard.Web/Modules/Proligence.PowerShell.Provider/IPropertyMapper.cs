namespace Proligence.PowerShell.Provider
{
    /// <summary>
    /// Maps property values between two objects. This class can be used to easily copy cmdlet parameters onto an
    /// existing object.
    /// </summary>
    public interface IPropertyMapper
    {
        /// <summary>
        /// Maps properties from the source object to the target object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        void MapProperties(object source, object target);
    }
}