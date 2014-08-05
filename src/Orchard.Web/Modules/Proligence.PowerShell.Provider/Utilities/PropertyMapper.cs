namespace Proligence.PowerShell.Provider.Utilities
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Maps property values between two objects. This class can be used to easily copy cmdlet parameters onto an
    /// existing object.
    /// </summary>
    public class PropertyMapper : IPropertyMapper
    {
        /// <summary>
        /// The default instance of the <see cref="IPropertyMapper"/>.
        /// </summary>
        private static readonly IPropertyMapper DefaultInstance = new PropertyMapper();

        /// <summary>
        /// Gets the default instance of the <see cref="IPropertyMapper"/>.
        /// </summary>
        public static IPropertyMapper Instance
        {
            get { return DefaultInstance; }
        }

        /// <summary>
        /// Maps properties from the source object to the target object.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="target">The target object.</param>
        public void MapProperties(object source, object target)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            PropertyInfo[] sourceProperties = source.GetType().GetProperties();
            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                PropertyInfo targetProperty = target.GetType().GetProperty(sourceProperty.Name);
                if ((targetProperty != null) && this.ShouldMap(source, sourceProperty, target, targetProperty))
                {
                    this.MapProperty(source, sourceProperty, target, targetProperty);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified properties should be mapped.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="sourceProperty">The source property.</param>
        /// <param name="target">The target object.</param>
        /// <param name="targetProperty">The target property.</param>
        /// <returns><c>true</c> if the properties should be mapped; otherwise, <c>false</c>.</returns>
        protected virtual bool ShouldMap(object source, PropertyInfo sourceProperty, object target, PropertyInfo targetProperty)
        {
            var sourceAttr = (MappableAttribute)Attribute.GetCustomAttribute(sourceProperty, typeof(MappableAttribute));
            var targetAttr = (MappableAttribute)Attribute.GetCustomAttribute(targetProperty, typeof(MappableAttribute));

            // Do not map if the source property doesn't have the Mappable attribute or Mappable is false
            if ((sourceAttr == null) || !sourceAttr.Mappable)
            {
                return false;
            }

            // Do not map if the target property has the Mappable attribute and Mappable is false
            if ((targetAttr != null) && !targetAttr.Mappable)
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Maps the specified property.
        /// </summary>
        /// <param name="source">The source object.</param>
        /// <param name="sourceProperty">The source property to map.</param>
        /// <param name="target">The target object.</param>
        /// <param name="targetProperty">The target property to map.</param>
        protected virtual void MapProperty(object source, PropertyInfo sourceProperty, object target, PropertyInfo targetProperty)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (sourceProperty == null)
            {
                throw new ArgumentNullException("sourceProperty");
            }

            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (targetProperty == null)
            {
                throw new ArgumentNullException("targetProperty");
            }

            if (sourceProperty.CanRead && targetProperty.CanWrite)
            {
                targetProperty.SetValue(target, sourceProperty.GetValue(source));
            }
        }
    }
}