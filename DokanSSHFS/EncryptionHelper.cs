using System;
using System.Security.Cryptography;
using System.Text;

namespace DokanSSHFS
{
    public class EncryptionHelper
    {
        private static readonly byte[] entropy = { 1, 2, 3, 4, 1, 2, 3, 4 };

        public static string EncryptToBase64String(string plain)
        {
            var buffer = Encoding.Unicode.GetBytes(plain);
            var encryptedPw = ProtectedData.Protect(buffer, entropy, DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(encryptedPw);
        }

        public static string DecryptBase64String(string encrypted)
        {
            try
            {
                var bytes = Convert.FromBase64String(encrypted);
                var decrypted = ProtectedData.Unprotect(bytes, entropy, DataProtectionScope.LocalMachine);
                return Encoding.Unicode.GetString(decrypted);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
