using CartotekaApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CartotekaApp.Domain
{
    /// <summary>
    /// ViewModel для главного окна приложения.
    /// </summary>
    public class MainWindowViewModel : ObservableObject
    {
        private const string ADMIN_ROLE = "Администратор";
        private readonly AuthService _authService;
        private readonly ICartotekaDbContext _dbContext;
        private MenuItem _selectedItem;
        private int _selectedIndex;
        private bool _isSideMenuOpen;

        public bool IsSideMenuOpen
        {
            get => _isSideMenuOpen;
            set => SetProperty(ref _isSideMenuOpen, value);
        }

        /// <summary>
        /// Выбранный элемент меню.
        /// </summary>
        public MenuItem SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        /// <summary>
        /// Индекс выбранного элемента меню.
        /// </summary>
        public int SelectedIndex
        {
            get => _selectedIndex;
            set => SetProperty(ref _selectedIndex, value);
        }

        /// <summary>
        /// Коллекция пунктов меню.
        /// </summary>
        public ObservableCollection<MenuItem> MenuItems { get; }

        /// <summary>
        /// Текущий пользователь.
        /// </summary>
        public User CurrentUser { get; }

        /// <summary>
        /// Команда выхода из приложения.
        /// </summary>
        public ICommand LogoutCommand { get; }

        public MainWindowViewModel(ICartotekaDbContext dbContext, AuthService authService)
        {
            _authService = authService;
            _dbContext = dbContext;

            CurrentUser = _authService.CurrentUser;

            // Инициализация коллекции пунктов меню
            MenuItems = new ObservableCollection<MenuItem>(new[]
            {
                new MenuItem("Главная", typeof(HomePage), new HomeViewModel(dbContext, CurrentUser)),
                new MenuItem("Книги", typeof(BooksPage), new BooksViewModel(dbContext)),
                new MenuItem("Руководство пользователя", typeof(UserGuidePage), this), // Передаем как контекст эту ViewModel для чтения свойства IsSideMenuOpen
            });

            // Если текущий пользователь является администратором, добавляем пункт меню "Регистрация"
            if (CurrentUser.Role.RoleName == ADMIN_ROLE)
            {
                MenuItems.Add(new MenuItem("Регистрация", typeof(RegisterPage), new RegisterViewModel(dbContext, _authService)));
            }

            MenuItems.Add(new MenuItem("О программе", typeof(AboutPage)));

            SelectedItem = MenuItems.First();

            // Команда выхода из приложения
            LogoutCommand = new RelayCommand(_ =>
            {
                _authService.Logout();
                OpenAuthWindow();
                CloseMainWindow();
            });
        }

        /// <summary>
        /// Открытие окна авторизации.
        /// </summary>
        private void OpenAuthWindow()
        {
            var authWindow = new AuthWindow
            {
                DataContext = new AuthWindowViewModel(_dbContext)
            };
            authWindow.Show();
        }

        /// <summary>
        /// Событие запроса закрытия окна.
        /// </summary>
        public event EventHandler CloseWindowRequested;

        /// <summary>
        /// Закрытие главного окна.
        /// </summary>
        private void CloseMainWindow()
        {
            CloseWindowRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
