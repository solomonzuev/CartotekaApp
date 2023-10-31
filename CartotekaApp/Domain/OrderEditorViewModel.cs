using CartotekaApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace CartotekaApp.Domain
{
    /// <summary>
    /// ViewModel для редактора заказа.
    /// </summary>
    public class OrderEditorViewModel : ObservableObject
    {
        private readonly ICartotekaDbContext _dbContext;
        private Book _selectedBook;
        private string _errorMessage;
        private string _successMessage;
        private object _bookEditorDataContext;
        private bool _isBookEditorOpen;
        private ObservableCollection<Book> _books;

        public bool IsBookEditorOpen
        {
            get => _isBookEditorOpen;
            set => SetProperty(ref _isBookEditorOpen, value);
        }
        public object BookEditorDataContext
        {
            get => _bookEditorDataContext;
            set => SetProperty(ref _bookEditorDataContext, value);
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
        /// Сообщение об успешном сохранении данных.
        /// </summary>
        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        /// <summary>
        /// Выбранная книга.
        /// </summary>
        public Book SelectedBook
        {
            get => _selectedBook;
            set => SetProperty(ref _selectedBook, value);
        }

        /// <summary>
        /// Коллекция книг.
        /// </summary>
        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        /// <summary>
        /// Текущий заказ.
        /// </summary>
        public Order CurrentOrder { get; }

        /// <summary>
        /// Команда сохранения изменений данных о закупке.
        /// </summary>
        public ICommand SaveCommand { get; }

        /// <summary>
        /// Команда закрытия окна редактирования данных о закупке.
        /// </summary>
        public ICommand CloseCommand { get; }
        public ICommand AddCommand { get; }

        /// <summary>
        /// Конструктор класса OrderEditorViewModel.
        /// </summary>
        /// <param name="dbContext">Контекст базы данных.</param>
        /// <param name="order">Заказ.</param>
        /// <param name="closeMethod">Метод отмены.</param>
        public OrderEditorViewModel(ICartotekaDbContext dbContext, Order order, Action closeMethod)
        {
            _dbContext = dbContext;
            CurrentOrder = order;

            SaveCommand = new RelayCommand(_ => SaveOrder());
            AddCommand = new RelayCommand(_ => AddBook());
            CloseCommand = new RelayCommand(_ => closeMethod());

            LoadBooks();
            SelectedBook = order.Book;

            BookEditorDataContext = new BookEditorViewModel(_dbContext, new Book(), CloseBookEditor);
        }

        private void LoadBooks()
        {
            Books = new ObservableCollection<Book>(_dbContext.Books);
        }

        private void AddBook()
        {
            BookEditorDataContext = new BookEditorViewModel(_dbContext, new Book(), CloseBookEditor);
            IsBookEditorOpen = true;
        }

        private void CloseBookEditor()
        {
            IsBookEditorOpen = false;
            BookEditorDataContext = null;
            LoadBooks();
        }

        /// <summary>
        /// Сохраняет данные о закупке.
        /// </summary>
        /// <remarks>
        /// Проверяет валидность заказа с помощью метода <see cref="ValidateOrder"/>.
        /// Если заказ является валидным, сохраняет его в базе данных.
        /// В случае ошибки при сохранении, выводит сообщение об ошибке с помощью диалогового окна.
        /// </remarks>
        private void SaveOrder()
        {
            ValidateOrder();

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                return;
            }

            CurrentOrder.Book = SelectedBook;

            if (CurrentOrder.Id == 0)
            {
                _dbContext.Orders.Add(CurrentOrder);
            }

            try
            {
                _dbContext.SaveChanges();

                ErrorMessage = string.Empty;
                SuccessMessage = "Данные сохранены!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при сохранении", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Проверяет валидность данных закупки.
        /// </summary>
        private void ValidateOrder()
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedBook == null)
            {
                ErrorMessage = "Не выбрана книга, закупка которой была выполнена!";
            }
            else if (CurrentOrder.OrderYear < 2000 || CurrentOrder.OrderYear > 2100)
            {
                ErrorMessage = "Год закупки должен быть в диапазоне от 2000 до 2100 года!";
            }
            else if (CurrentOrder.Price < 0)
            {
                ErrorMessage = "Цена не может быть отрицательной!";
            }
            else if (CurrentOrder.Units <= 0)
            {
                ErrorMessage = "Количество закупаемых книг должно быть больше 0!";
            }
        }
    }
}
