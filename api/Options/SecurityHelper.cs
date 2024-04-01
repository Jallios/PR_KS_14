using System.Security.Cryptography;
using System.Text;

namespace api.Options
{
    public class SecurityHelper
    {
        public static string generateSalt(int nSalt)
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[nSalt];
            rng.GetBytes(salt);

            return Convert.ToHexString(salt);
        }

        public static string hashPassword(string password, string salt)
        {
            using SHA512 hash = SHA512.Create();
            return Convert.ToHexString(hash.ComputeHash(Encoding.UTF8.GetBytes(password + salt)));
        }

        public static bool verifypassword(string password, string salt, string hashedPassword)
        {
            string hash = hashPassword(password, salt);
            return hash.Equals(hashedPassword);
        }
    }
}
