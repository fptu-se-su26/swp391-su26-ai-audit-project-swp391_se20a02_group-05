using System;
using System.Text;
using System.Text.RegularExpressions;

namespace CVerify.API.Modules.Shared.System.Extensions
{
    /// <summary>
    /// Provides utility extension methods for string sanitization, security escaping, and casing formats.
    /// </summary>
    public static class StringSanitizationExtensions
    {
        private static readonly Regex HtmlTagRegex = new Regex("<[^>]*>", RegexOptions.Compiled);

        /// <summary>
        /// Strips all HTML tags from the given string.
        /// Useful for sanitizing descriptions, comments, or inputs.
        /// </summary>
        /// <param name="input">The HTML text input.</param>
        /// <returns>A plain text representation of the input without HTML markup.</returns>
        public static string StripHtml(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            return HtmlTagRegex.Replace(input, string.Empty).Trim();
        }

        /// <summary>
        /// Escapes SQL wildcard characters ('%', '_', '[') to make them safe for use inside a SQL LIKE search.
        /// </summary>
        /// <param name="input">The search keyword input.</param>
        /// <returns>The escaped search string.</returns>
        public static string EscapeSqlLikeWildcards(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (c == '%' || c == '_' || c == '[')
                {
                    sb.Append('[').Append(c).Append(']');
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts the input string to snake_case formatting.
        /// </summary>
        /// <param name="input">The string to format (e.g. CamelCase or PascalCase).</param>
        /// <returns>The snake_cased string.</returns>
        public static string ToSnakeCase(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsUpper(c))
                {
                    if (i > 0 && input[i - 1] != '_' && !char.IsUpper(input[i - 1]))
                    {
                        sb.Append('_');
                    }
                    sb.Append(char.ToLowerInvariant(c));
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Converts the input string to camelCase formatting.
        /// Handles snake_case, PascalCase, or mixed inputs.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The camelCased string.</returns>
        public static string ToCamelCase(this string? input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            string clean = input.Replace("_", " ");
            var words = clean.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            
            // First word lowercase
            string firstWord = words[0];
            sb.Append(char.ToLowerInvariant(firstWord[0]));
            if (firstWord.Length > 1)
            {
                sb.Append(firstWord.Substring(1));
            }

            // Subsequent words capitalized
            for (int i = 1; i < words.Length; i++)
            {
                string word = words[i];
                sb.Append(char.ToUpperInvariant(word[0]));
                if (word.Length > 1)
                {
                    sb.Append(word.Substring(1).ToLowerInvariant());
                }
            }

            return sb.ToString();
        }
    }
}
