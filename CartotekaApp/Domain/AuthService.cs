using CartotekaApp.Models;
using System.Linq;

namespace CartotekaApp.Domain
{
    /// <summary>
    /// Сервис аутентификации.
    /// </summary>
    public class AuthService
    {
        private const string ADMIN_ROLE = "Администратор";
        private readonly ICartotekaDbContext _dbContext;

        /// <summary>
        /// Текущий аутентифицированный пользователь.
        /// </summary>
        public User CurrentUser { get; private set; }

        /// <summary>
        /// Флаг, указывающий, прошла ли аутентификация.
        /// </summary>
        public bool IsAuthenticated => CurrentUser != null;

        /// <summary>
        /// Конструктор класса AuthService.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public AuthService(ICartotekaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Аутентификация пользователя.
        /// </summary>
        /// <param name="username">Имя пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <param name="role">Роль пользователя.</param>
        /// <returns>True, если аутентификация успешна, иначе False.</returns>
        public bool AuthenticateUser(string username, string password, Role role)
        {
            var passwordHash = SecurityPassword.HashPassword(password);

            CurrentUser = _dbContext.Users.SingleOrDefault(u => u.UserName == username && u.PasswordHash == passwordHash && u.Role.RoleName == role.RoleName);

            return IsAuthenticated;
        }

        /// <summary>
        /// Регистрация нового пользователя.
        /// </summary>
        /// <param name="imya">Имя пользователя.</param>
        /// <param name="familiya">Фамилия пользователя.</param>
        /// <param name="otchestvo">Отчество пользователя.</param>
        /// <param name="username">Имя пользователя.</param>
        /// <param name="password">Пароль пользователя.</param>
        /// <param name="role">Роль пользователя.</param>
        /// <returns>True, если регистрация успешна, иначе False.</returns>
        public bool RegisterUser(string imya, string familiya, string otchestvo, string username, string password, Role role)
        {
            if (CurrentUser != null && CurrentUser.Role.RoleName == ADMIN_ROLE)
            {
                var isUserExists = _dbContext.Users.Any(u => u.UserName == username);

                if (!isUserExists)
                {
                    var user = new User()
                    {
                        Imya = imya,
                        Familiya = familiya,
                        Otchestvo = otchestvo,
                        UserName = username,
                        PasswordHash = SecurityPassword.HashPassword(password),
                        Role = role
                    };

                    _dbContext.Users.Add(user);
                    _dbContext.SaveChanges();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Выход из системы.
        /// </summary>
        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
