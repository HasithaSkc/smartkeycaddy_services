
using System.Security.Cryptography;
using System.Text;

namespace SmartKeyCaddy.Common
{
    public class PasswordHash
    {
        const int _keySize = 64;
        const int _iterations = 350000;
        static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        const string _sault = "api_admin_sault";

        public static string HashPasword(string password)
        {
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
            Encoding.UTF8.GetBytes(_sault),
            _iterations,
                hashAlgorithm,
                _keySize);
            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, _iterations, hashAlgorithm, _keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
}