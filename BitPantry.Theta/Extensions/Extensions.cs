using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace BitPantry.Theta.Extensions
{
    /// <summary>
    /// Helper extensions used internally to support the Console and may be used externally to support commands
    /// </summary>
    static class Extensions
    {
        #region STRING EXTENSIONS

        /// <summary>
        /// Splits the string while preserving quoted values (i.e. instances of the delimiter character inside of quotes will not be split apart).
        /// Trims leading and trailing whitespace from the individual string values.
        /// Does not include empty values.
        /// </summary>
        /// <param name="value">The string to be split.</param>
        /// <param name="delimiter">The delimiter to use to split the string, e.g. ',' for CSV.</param>
        /// <returns>A collection of individual strings parsed from the original value.</returns>
        internal static IEnumerable<string> SplitWhilePreservingQuotedValues(this string value, char delimiter)
        {

            // this one allows for both single and double quotes [^\s"']+|"([^"]*)"|'([^']*)'
            Regex csvPreservingQuotedStrings = new Regex(string.Format("(\"[^\"]*\"|[^{0}])+(\\s?)+", delimiter));
            var values =
                csvPreservingQuotedStrings.Matches(value)
                .Cast<Match>()
                .Select(m => m.Value.Trim().Trim('"'))
                .Where(v => !string.IsNullOrWhiteSpace(v));
            return values;
        }

        #endregion
    }
}
