using System.Security.Cryptography;
using System.Text;

namespace CartotekaApp.Domain
{
    /// <summary>
    /// Класс для обеспечения безопасности паролей.
    /// </summary>
    public static class SecurityPassword
    {
        /// <summary>
        /// Хеширует пароль с использованием алгоритма SHA256.
        /// </summary>
        /// <param name="password">Пароль для хеширования.</param>
        /// <returns>Хеш-значение пароля.</returns>
        public static string HashPassword(string password)
        {
            var passwordBytes = Encoding.UTF8.GetBytes(password);

            using (var sha256 = SHA256.Create())
            {
                var passwordHash = sha256.ComputeHash(passwordBytes);

                var sb = new StringBuilder();

                foreach (var b in passwordHash)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
