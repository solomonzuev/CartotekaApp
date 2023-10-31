using CartotekaApp.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace CartotekaApp.Domain
{
    /// <summary>
    /// ViewModel для регистрации нового пользователя.
    /// </summary>
    public class RegisterViewModel : ObservableObject
    {
        private string _imya;
        private string _familiya;
        private string _otchestvo;
        private string _username;
        private string _password;
        private string _confirmPassword;
        private string _notificationMessage;
        private Brush _notificationForeground;
        private readonly ICartotekaDbContext _dbContext;
        private readonly AuthService _authService;
        private Role _selectedRole;

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Imya
        {
            get => _imya;
            set
            {
                SetProperty(ref _imya, value);
                ClearNotification();
            }
        }

        /// <summary>
        /// Фамилия пользователя.
        /// </summary>
        public string Familiya
        {
            get => _familiya;
            set
            {
                SetProperty(ref _familiya, value);
                ClearNotification();
            }
        }

        /// <summary>
        /// Отчество пользователя.
        /// </summary>
        public string Otchestvo
        {
            get => _otchestvo;
            set
            {
                SetProperty(ref _otchestvo, value);
                ClearNotification();
            }
        }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                SetProperty(ref _username, value);
                ClearNotification();
            }
        }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                SetProperty(ref _password, value);
                ClearNotification();
            }
        }

        /// <summary>
        /// Подтверждение пароля пользователя.
        /// </summary>
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                SetProperty(ref _confirmPassword, value);
                ClearNotification();
            }
        }

        /// <summary>
        /// Текст уведомления.
        /// </summary>
        public string NotificationMessage
        {
            get => _notificationMessage;
            set => SetProperty(ref _notificationMessage, value);
        }

        /// <summary>
        /// Цвет текста уведомления.
        /// </summary>
        public Brush NotificationForeground
        {
            get => _notificationForeground;
            set => SetProperty(ref _notificationForeground, value);
        }

        /// <summary>
        /// Выбранная роль пользователя.
        /// </summary>
        public Role SelectedRole
        {
            get => _selectedRole;
            set
            {
                SetProperty(ref _selectedRole, value);
                ClearNotification();
            }
        }

        /// <summary>
        /// Список ролей пользователей.
        /// </summary>
        public ObservableCollection<Role> Roles { get; }

        /// <summary>
        /// Команда регистрации пользователя.
        /// </summary>
        public ICommand RegisterCommand { get; }

        /// <summary>
        /// Конструктор класса RegisterViewModel.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="authService">Сервис аутентификации.</param>
        public RegisterViewModel(ICartotekaDbContext dbContext, AuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;

            RegisterCommand = new RelayCommand(_ => Register()); // Инициализация команды регистрации

            Roles = new ObservableCollection<Role>(_dbContext.Roles); // Загрузка списка ролей
            SelectedRole = Roles.First(); // Выбор первой роли по умолчанию
        }

        /// <summary>
        /// Очищает уведомление и его цвет
        /// </summary>
        private void ClearNotification()
        {
            NotificationMessage = string.Empty;
            NotificationForeground = default;
        }

        /// <summary>
        /// Метод для регистрации пользователя.
        /// </summary>
        private void Register()
        {
            ValidateUserData(); // Проверка введенных данных пользователя

            if (!string.IsNullOrWhiteSpace(NotificationMessage))
            {
                return;
            }

            var isRegistered = _authService.RegisterUser(Imya, Familiya, Otchestvo, Username, Password, SelectedRole);

            if (isRegistered)
            {
                ClearProperties();
                SetSuccess("Регистрация прошла успешно!");
            }
            else
            {
                SetFail("Не удалось зарегистрировать пользователя!");
            }
        }

        /// <summary>
        /// Устанавливает текст для успешного сообщения
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public void SetSuccess(string message)
        {
            NotificationMessage = message;
            NotificationForeground = new SolidColorBrush(Color.FromRgb(0, 128, 0)); // зелёный цвет
        }

        /// <summary>
        /// Устанавливает текст для неудачного сообщения
        /// </summary>
        /// <param name="message">Текст сообщения</param>
        public void SetFail(string message)
        {
            NotificationMessage = message;
            NotificationForeground = new SolidColorBrush(Color.FromRgb(255, 0, 0)); // красный цвет
        }

        /// <summary>
        /// Очищает значения свойств, связанных с данными пользователя.
        /// </summary>
        private void ClearProperties()
        {
            Imya = string.Empty;
            Familiya = string.Empty;
            Otchestvo = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
        }

        /// <summary>
        /// Проверка введенных данных пользователя.
        /// </summary>
        private void ValidateUserData()
        {
            if (string.IsNullOrWhiteSpace(Imya))
            {
                SetFail("Имя не может быть пустым!");
            }
            else if (string.IsNullOrWhiteSpace(Familiya))
            {
                SetFail("Фамилия не может быть пустой!");
            }
            else if (string.IsNullOrWhiteSpace(Otchestvo))
            {
                SetFail("Отчество не может быть пустым!");
            }
            else if (string.IsNullOrWhiteSpace(Username))
            {
                SetFail("Имя пользователя не может быть пустым!");
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                SetFail("Пароль не может быть пустым!");
            }
            else if (Password != ConfirmPassword)
            {
                SetFail("Пароль и подтверждение пароля не совпадают!");
            }
            else
            {
                SetFail(string.Empty);
            }
        }
    }
}
