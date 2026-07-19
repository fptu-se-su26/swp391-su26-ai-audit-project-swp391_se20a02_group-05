using System;
using System.Security.Cryptography;
using System.Text;

namespace CVerify.API.Modules.Shared.Security
{
    /// <summary>
    /// Provides advanced, enterprise-grade cryptographic operations.
    /// Includes secure salting, SHA-2 hashing, constant-time HMAC validation, and URL-safe Base64 conversions.
    /// </summary>
    public static class AdvancedCryptoHelper
    {
        /// <summary>
        /// Generates a cryptographically secure random salt of the specified size.
        /// </summary>
        /// <param name="size">The size of the salt in bytes. Defaults to 32 bytes (256 bits).</param>
        /// <returns>A base64 encoded string representing the salt.</returns>
        public static string GenerateSecureSalt(int size = 32)
        {
            if (size <= 0)
            {
                throw new ArgumentException("Salt size must be greater than zero.", nameof(size));
            }

            byte[] saltBytes = new byte[size];
            RandomNumberGenerator.Fill(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        /// <summary>
        /// Computes the SHA-256 hash of a plain text string combined with a base64 encoded salt.
        /// </summary>
        /// <param name="plainText">The string to hash.</param>
        /// <param name="saltBase64">The base64 encoded salt string.</param>
        /// <returns>A hex encoded representation of the hash.</returns>
        public static string ComputeSha256Hash(string plainText, string saltBase64)
        {
            if (plainText == null) throw new ArgumentNullException(nameof(plainText));
            if (saltBase64 == null) throw new ArgumentNullException(nameof(saltBase64));

            byte[] textBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] saltBytes = Convert.FromBase64String(saltBase64);

            byte[] combinedBytes = new byte[textBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(textBytes, 0, combinedBytes, saltBytes.Length, textBytes.Length);

            byte[] hashBytes = SHA256.HashData(combinedBytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        /// <summary>
        /// Computes the SHA-512 hash of a plain text string combined with a base64 encoded salt.
        /// </summary>
        /// <param name="plainText">The string to hash.</param>
        /// <param name="saltBase64">The base64 encoded salt string.</param>
        /// <returns>A hex encoded representation of the hash.</returns>
        public static string ComputeSha512Hash(string plainText, string saltBase64)
        {
            if (plainText == null) throw new ArgumentNullException(nameof(plainText));
            if (saltBase64 == null) throw new ArgumentNullException(nameof(saltBase64));

            byte[] textBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] saltBytes = Convert.FromBase64String(saltBase64);

            byte[] combinedBytes = new byte[textBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(textBytes, 0, combinedBytes, saltBytes.Length, textBytes.Length);

            byte[] hashBytes = SHA512.HashData(combinedBytes);
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        /// <summary>
        /// Computes a HMAC-SHA256 signature for the given payload data.
        /// </summary>
        /// <param name="data">The byte array payload data.</param>
        /// <param name="key">The key used for signing.</param>
        /// <returns>The signature byte array.</returns>
        public static byte[] ComputeHmacSha256(byte[] data, byte[] key)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (key == null) throw new ArgumentNullException(nameof(key));

            using (var hmac = new HMACSHA256(key))
            {
                return hmac.ComputeHash(data);
            }
        }

        /// <summary>
        /// Verifies a HMAC-SHA256 signature using constant-time comparison to prevent timing attacks.
        /// </summary>
        /// <param name="data">The byte array payload data.</param>
        /// <param name="key">The key used for signing.</param>
        /// <param name="signature">The signature to verify.</param>
        /// <returns>True if the signature is valid; otherwise false.</returns>
        public static bool VerifyHmacSha256(byte[] data, byte[] key, byte[] signature)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            if (key == null) throw new ArgumentNullException(nameof(key));
            if (signature == null) throw new ArgumentNullException(nameof(signature));

            byte[] computedHash = ComputeHmacSha256(data, key);

            // Prevent timing attacks using fixed-time comparison
            return CryptographicOperations.FixedTimeEquals(computedHash, signature);
        }

        /// <summary>
        /// Encodes a byte array to a URL-safe Base64 string (RFC 4648).
        /// Removes padding character '=' and replaces '+' with '-' and '/' with '_'.
        /// </summary>
        /// <param name="input">The byte array to encode.</param>
        /// <returns>The URL-safe base64 string.</returns>
        public static string Base64UrlEncode(byte[] input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            string base64 = Convert.ToBase64String(input);
            return base64.Replace("=", string.Empty).Replace('+', '-').Replace('/', '_');
        }

        /// <summary>
        /// Decodes a URL-safe Base64 string back to a byte array.
        /// Replaces URL-safe characters and adds correct padding.
        /// </summary>
        /// <param name="input">The URL-safe base64 encoded string.</param>
        /// <returns>The decoded byte array.</returns>
        public static byte[] Base64UrlDecode(string input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));

            string base64 = input.Replace('-', '+').Replace('_', '/');
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            return Convert.FromBase64String(base64);
        }
    }
}
