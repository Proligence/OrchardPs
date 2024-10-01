using System;
using System.Reflection;
using Orchard.Validation;

namespace Proligence.PowerShell.Provider.Utilities {
    /// <summary>
    /// Maps property values between two objects. This class can be used to easily copy cmdlet parameters onto an
    /// existing object.
    /// </summary>
    public class PropertyMapper : IPropertyMapper {
        /// <summary>
        /// The default instance of the <see cref="IPropertyMapper"/>.
        /// </summary>
        private static readonly IPropertyMapper _defaultInstance = new PropertyMapper();

        /// <summary>
        /// Gets the default instance of the <see cref="IPropertyMapper"/>.
        /// </summary>
        public static IPropertyMapper Instance {
            get { return _defaultInstance; }
        }

        /// <summary>
        /// Maps properties from the source object to the target object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        public void MapProperties(object source, object target) {
            Argument.ThrowIfNull(source, "source");
            Argument.ThrowIfNull(target, "target");

            PropertyInfo[] sourceProperties = source.GetType().GetProperties();
            foreach (PropertyInfo sourceProperty in sourceProperties) {
                PropertyInfo targetProperty = target.GetType().GetProperty(sourceProperty.Name);
                if ((targetProperty != null) && ShouldMap(source, sourceProperty, target, targetProperty)) {
                    MapProperty(source, sourceProperty, target, targetProperty);
                }
            }
        }

        protected virtual bool ShouldMap(object source, PropertyInfo sourceProperty, object target,
            PropertyInfo targetProperty) {
            var sourceAttr =
                (MappableAttribute) Attribute.GetCustomAttribute(sourceProperty, typeof (MappableAttribute));
            var targetAttr =
                (MappableAttribute) Attribute.GetCustomAttribute(targetProperty, typeof (MappableAttribute));

            // Do not map if the source property doesn't have the Mappable attribute or Mappable is false
            if ((sourceAttr == null) || !sourceAttr.Mappable) {
                return false;
            }

            // Do not map if the target property has the Mappable attribute and Mappable is false
            if ((targetAttr != null) && !targetAttr.Mappable) {
                return false;
            }

            return true;
        }

        protected virtual void MapProperty(object source, PropertyInfo sourceProperty, object target,
            PropertyInfo targetProperty) {
            Argument.ThrowIfNull(source, "source");
            Argument.ThrowIfNull(sourceProperty, "sourceProperty");
            Argument.ThrowIfNull(target, "target");
            Argument.ThrowIfNull(targetProperty, "targetProperty");

            if (sourceProperty.CanRead && targetProperty.CanWrite) {
                targetProperty.SetValue(target, sourceProperty.GetValue(source));
            }
        }
    }
}