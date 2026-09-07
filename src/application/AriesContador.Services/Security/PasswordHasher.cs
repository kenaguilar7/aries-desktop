using System;
using System.Security.Cryptography;
using System.Text;

namespace AriesContador.Services.Security
{
    /// <summary>
    /// PBKDF2-HMAC-SHA256 portable a net48 y netstandard2.0.
    /// Formato: pbkdf2$iter$saltHashBase64
    /// </summary>
    public static class PasswordHasher
    {
        public const string Prefix = "pbkdf2$";
        public const int DefaultIterations = 100000;
        private const int SaltSize = 16;
        private const int KeySize = 32;

        public static bool LooksHashed(string stored)
        {
            return !string.IsNullOrWhiteSpace(stored)
                   && stored.StartsWith(Prefix, StringComparison.Ordinal);
        }

        public static string Hash(string password, int iterations = DefaultIterations)
        {
            if (password == null)
                throw new ArgumentNullException(nameof(password));

            var salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
                rng.GetBytes(salt);

            var hash = Pbkdf2(password, salt, iterations);
            return $"{Prefix}{iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public static bool Verify(string password, string stored)
        {
            if (string.IsNullOrEmpty(password) || !TryParse(stored, out var iterations, out var salt, out var expected))
                return false;

            var actual = Pbkdf2(password, salt, iterations);
            return FixedTimeEquals(actual, expected);
        }

        public static bool TryParse(string stored, out int iterations, out byte[] salt, out byte[] hash)
        {
            iterations = 0;
            salt = null;
            hash = null;
            if (!LooksHashed(stored))
                return false;

            var parts = stored.Split('$');
            if (parts.Length != 4)
                return false;

            if (!int.TryParse(parts[1], out iterations) || iterations <= 0)
                return false;

            try
            {
                salt = Convert.FromBase64String(parts[2]);
                hash = Convert.FromBase64String(parts[3]);
            }
            catch (FormatException)
            {
                return false;
            }

            return salt.Length > 0 && hash.Length > 0;
        }

        private static byte[] Pbkdf2(string password, byte[] salt, int iterations)
        {
            var pwd = Encoding.UTF8.GetBytes(password);
            using (var hmac = new HMACSHA256(pwd))
            {
                var hashLen = hmac.HashSize / 8;
                var blockCount = (KeySize + hashLen - 1) / hashLen;
                var output = new byte[blockCount * hashLen];
                var saltBlock = new byte[salt.Length + 4];
                Buffer.BlockCopy(salt, 0, saltBlock, 0, salt.Length);

                for (var i = 1; i <= blockCount; i++)
                {
                    saltBlock[salt.Length] = (byte)(i >> 24);
                    saltBlock[salt.Length + 1] = (byte)(i >> 16);
                    saltBlock[salt.Length + 2] = (byte)(i >> 8);
                    saltBlock[salt.Length + 3] = (byte)i;

                    var u = hmac.ComputeHash(saltBlock);
                    var t = (byte[])u.Clone();
                    for (var j = 1; j < iterations; j++)
                    {
                        u = hmac.ComputeHash(u);
                        for (var k = 0; k < t.Length; k++)
                            t[k] ^= u[k];
                    }

                    Buffer.BlockCopy(t, 0, output, (i - 1) * hashLen, hashLen);
                }

                var key = new byte[KeySize];
                Buffer.BlockCopy(output, 0, key, 0, KeySize);
                return key;
            }
        }

        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            var diff = 0;
            for (var i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}
