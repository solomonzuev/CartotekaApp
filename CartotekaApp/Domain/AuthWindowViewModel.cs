using CartotekaApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CartotekaApp.Domain
{
    /// <summary>
    /// ViewModel для окна аутентификации.
    /// </summary>
    public class AuthWindowViewModel : ObservableObject
    {
        private string _username;
        private string _password;
        private string _errorMessage;
        private Role _selectedRole;
        private readonly ICartotekaDbContext _dbContext;
        private readonly AuthService _authService;

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        /// <summary>
        /// Пароль пользователя.
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        /// <summary>
        /// Выбранная роль.
        /// </summary>
        public Role SelectedRole
        {
            get => _selectedRole;
            set => SetProperty(ref _selectedRole, value);
        }

        /// <summary>
        /// Список ролей.
        /// </summary>
        public ObservableCollection<Role> Roles { get; }

        /// <summary>
        /// Команда аутентификации.
        /// </summary>
        public ICommand AuthenticateCommand { get; }

        /// <summary>
        /// Конструктор класса AuthWindowViewModel.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        public AuthWindowViewModel(ICartotekaDbContext dbContext)
        {
            _dbContext = dbContext;
            _authService = new AuthService(_dbContext);

            // Команда аутентификации пользователя
            AuthenticateCommand = new RelayCommand(_ => Authenticate());

            Roles = new ObservableCollection<Role>(_dbContext.Roles); // Загрузка списка ролей
            SelectedRole = Roles.First();
        }

        /// <summary>
        /// Метод аутентификации пользователя.
        /// </summary>
        public void Authenticate()
        {
            ValidateInput();

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                return;
            }

            var isAuthenticated = _authService.AuthenticateUser(Username, Password, SelectedRole);

            if (isAuthenticated)
            {
                ErrorMessage = string.Empty;
                OpenMainWindow();
                CloseAuthWindow();
            }
            else
            {
                ErrorMessage = "Неверное имя пользователя или пароль!";
            }
        }

        /// <summary>
        /// Проверка ввода данных.
        /// </summary>
        private void ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Имя пользователя не может быть пустым!";
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Пароль пользователя не может быть пустым!";
            }
            else
            {
                ErrorMessage = string.Empty;
            }
        }

        /// <summary>
        /// Открытие главного окна.
        /// </summary>
        private void OpenMainWindow()
        {
            var mainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(_dbContext, _authService)
            };

            mainWindow.Show();
        }

        public event EventHandler CloseWindowRequested;

        /// <summary>
        /// Закрытие окна аутентификации.
        /// </summary>
        private void CloseAuthWindow()
        {
            CloseWindowRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
