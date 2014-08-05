namespace Proligence.PowerShell.Provider.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a list of cmdlet arguments.
    /// </summary>
    [Serializable]
    public class ArgumentList : Dictionary<string, string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentList"/> class.
        /// </summary>
        public ArgumentList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgumentList"/> class with serialized data.
        /// </summary>
        /// <param name="info">
        /// A <see cref="SerializationInfo"/> object containing the information required to serialize the
        /// <see cref="ArgumentList"/>.
        /// </param>
        /// <param name="context">
        /// A <see cref="StreamingContext"/> structure containing the source and destination of the serialized stream
        /// associated with the <see cref="ArgumentList"/>.
        /// </param>
        protected ArgumentList(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Parses a list of cmdlet arguments.
        /// </summary>
        /// <param name="arguments">The arguments list.</param>
        /// <returns>A dictionary which maps parameter names to their values.</returns>
        public static ArgumentList Parse(ArrayList arguments)
        {
            if (arguments == null)
            {
                throw new ArgumentNullException("arguments");
            }

            var argumentList = new ArgumentList();

            IEnumerator<string> enumerator = arguments.Cast<string>().GetEnumerator();
            
            bool hasMoreArgs = enumerator.MoveNext();
            while (hasMoreArgs)
            {
                string name = ParseName(enumerator);
                if ((name != null) || (enumerator.Current == null))
                {
                    hasMoreArgs = enumerator.MoveNext();
                }

                string value = ParseValue(enumerator);
                if ((value != null) || (enumerator.Current == null))
                {
                    hasMoreArgs = enumerator.MoveNext();
                }

                if (!string.IsNullOrWhiteSpace(name))
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        argumentList[name] = value;
                    }
                    else
                    {
                        argumentList[name] = null;
                    }
                }
            }

            return argumentList;
        }

        private static string ParseName(IEnumerator<string> enumerator)
        {
            if ((enumerator.Current != null) && enumerator.Current.StartsWith("-", StringComparison.Ordinal))
            {
                return enumerator.Current.Substring(1);
            }

            return null;
        }

        private static string ParseValue(IEnumerator<string> enumerator)
        {
            if ((enumerator.Current == null) || !enumerator.Current.StartsWith("-", StringComparison.Ordinal))
            {
                return enumerator.Current;
            }

            return null;
        }
    }
}