using System;
using System.Text.RegularExpressions;

namespace CVerify.API.Modules.Shared.System.Extensions
{
    /// <summary>
    /// Provides advanced verification and validation extension methods for strings,
    /// focusing on security, structure, and integration inputs (such as Git URLs and email addresses).
    /// </summary>
    public static class ValidationExtensions
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\|\}~w])*)(?<=[0-9a-z])@))" +
            @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private static readonly Regex GitHubHttpsRegex = new Regex(
            @"^https?://(www\.)?github\.com/(?<owner>[\w\-.]+)/(?<repo>[\w\-.]+)(/|$)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex GitHubSshRegex = new Regex(
            @"^(git@)?github\.com:(?<owner>[\w\-.]+)/(?<repo>[\w\-.]+)(\.git)?$",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        /// <summary>
        /// Estimates the informational entropy of a password in bits, using the pool-size method.
        /// Entropy = Length * log2(Range). Higher entropy means greater cryptographic strength.
        /// </summary>
        /// <param name="password">The password to evaluate.</param>
        /// <returns>The calculated entropy in bits.</returns>
        public static double CalculatePasswordEntropy(this string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return 0.0;
            }

            int length = password.Length;
            int poolSize = 0;

            bool hasLower = false;
            bool hasUpper = false;
            bool hasDigit = false;
            bool hasSpecial = false;
            bool hasExtended = false;

            foreach (char c in password)
            {
                if (char.IsLower(c)) hasLower = true;
                else if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (char.IsSymbol(c) || char.IsPunctuation(c)) hasSpecial = true;
                else hasExtended = true;
            }

            if (hasLower) poolSize += 26;
            if (hasUpper) poolSize += 26;
            if (hasDigit) poolSize += 10;
            if (hasSpecial) poolSize += 33; // Standard ASCII symbols/punctuation
            if (hasExtended) poolSize += 100; // Estimated pool size for Unicode/extended characters

            if (poolSize == 0)
            {
                poolSize = 1;
            }

            return length * Math.Log2(poolSize);
        }

        /// <summary>
        /// Verifies whether the password meets enterprise-grade security standards:
        /// 1. Length of at least 8 characters.
        /// 2. Entropy score of at least 45 bits (moderate-to-strong threshold).
        /// </summary>
        /// <param name="password">The password to evaluate.</param>
        /// <returns>True if password meets criteria; otherwise false.</returns>
        public static bool IsSecurePassword(this string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                return false;
            }

            return password.CalculatePasswordEntropy() >= 45.0;
        }

        /// <summary>
        /// Validates string against strict email format guidelines.
        /// </summary>
        /// <param name="email">The email to validate.</param>
        /// <returns>True if valid email format; otherwise false.</returns>
        public static bool IsValidEmailSyntax(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            // Standard length boundary according to RFC 5321
            if (email.Length > 254)
            {
                return false;
            }

            try
            {
                return EmailRegex.IsMatch(email);
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        /// <summary>
        /// Parses a GitHub repository URL and extracts the owner and repository name.
        /// Supports standard HTTPS (with/without .git or subpaths) and SSH formats.
        /// </summary>
        /// <param name="url">The repository URL to parse.</param>
        /// <param name="owner">The extracted owner name, if valid.</param>
        /// <param name="repo">The extracted repository name, if valid.</param>
        /// <returns>True if the URL is a valid GitHub repository URL; otherwise false.</returns>
        public static bool TryParseGitHubUrl(this string url, out string owner, out string repo)
        {
            owner = string.Empty;
            repo = string.Empty;

            if (string.IsNullOrWhiteSpace(url))
            {
                return false;
            }

            string trimmedUrl = url.Trim();

            // Try HTTPS format
            var httpsMatch = GitHubHttpsRegex.Match(trimmedUrl);
            if (httpsMatch.Success)
            {
                owner = httpsMatch.Groups["owner"].Value;
                repo = httpsMatch.Groups["repo"].Value;

                if (repo.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                {
                    repo = repo.Substring(0, repo.Length - 4);
                }

                return !string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(repo);
            }

            // Try SSH format
            var sshMatch = GitHubSshRegex.Match(trimmedUrl);
            if (sshMatch.Success)
            {
                owner = sshMatch.Groups["owner"].Value;
                repo = sshMatch.Groups["repo"].Value;

                if (repo.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                {
                    repo = repo.Substring(0, repo.Length - 4);
                }

                return !string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(repo);
            }

            return false;
        }
    }
}
