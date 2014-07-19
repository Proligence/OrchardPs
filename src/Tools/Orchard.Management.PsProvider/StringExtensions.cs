namespace Orchard.Management.PsProvider
{
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Implements extension methods for the <see cref="string"/> class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Compares the specified string to another string which may contain wildcard (* and ?) characters.
        /// </summary>
        /// <param name="str">The <see cref="string"/> to compare.</param>
        /// <param name="pattern">The other <see cref="string"/> to compare.</param>
        /// <param name="caseSensitive">
        /// <c>true</c> to perform case-sensitive comparison; otherwise, <c>false</c>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <paramref name="str"/> is equal to or matches the <paramref name="pattern"/> pattern;
        /// otherwise, <c>false</c>.
        /// </returns>
        public static bool WildcardEquals(this string str, string pattern, bool caseSensitive = false)
        {
            if ((str == null) && (pattern == null))
            {
                return true;
            }

            if ((str == null) || (pattern == null))
            {
                return false;
            }

            if (!caseSensitive)
            {
                str = str.ToLower(CultureInfo.CurrentCulture);
                pattern = pattern.ToLower(CultureInfo.CurrentCulture);
            }
            
            return Regex.IsMatch(str, WildcardToRegex(pattern));
        }

        /// <summary>
        /// Converts the specified wildcard pattern to a regular expression.
        /// </summary>
        /// <param name="pattern">The pattern to convert.</param>
        /// <returns>The created regular expression.</returns>
        private static string WildcardToRegex(string pattern)
        {
            var regexBuilder = new StringBuilder((pattern ?? string.Empty).Length + 2);
            regexBuilder.Append("^");
            regexBuilder.Append(Regex.Escape(pattern ?? string.Empty));
            regexBuilder.Append("$");

            // Handle '*' wildcard
            regexBuilder.Replace("\\*", ".*");

            // Handle '?' wildcard
            regexBuilder.Replace("\\?", ".");
            
            return regexBuilder.ToString();
        }
    }
}