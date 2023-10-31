using CartotekaApp.Domain.Filters;
using CartotekaApp.Domain.Filters.Books;
using CartotekaApp.Domain.Interfaces;
using CartotekaApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CartotekaApp.Domain
{
    public class BooksViewModel : ObservableObject
    {
        private ObservableCollection<Book> _books;
        private Dictionary<string, IFilterStrategy<Book>> _filterStrategies;
        private readonly ICartotekaDbContext _dbContext;
        private string _selectedFilter;
        private string _searchText;
        private bool _isBookEditorOpen;

        private object _bookEditorDataContext;

        /// <summary>
        /// Контекст данных для окна добавления/редактирования данных о книгах
        /// </summary>
        public object BookEditorDataContext
        {
            get => _bookEditorDataContext;
            set => SetProperty(ref _bookEditorDataContext, value);
        }

        public bool IsBookEditorOpen
        {
            get => _isBookEditorOpen;
            set => SetProperty(ref _isBookEditorOpen, value);
        }

        public string SearchText
        {
            get => _searchText ?? string.Empty;
            set => SetProperty(ref _searchText, value);
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set => SetProperty(ref _selectedFilter, value);
        }

        public IEnumerable<string> Filters => _filterStrategies.Keys;

        public ObservableCollection<Book> Books
        {
            get => _books;
            set => SetProperty(ref _books, value);
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand PrintCommand { get; set; }

        public BooksViewModel(ICartotekaDbContext dbContext)
        {
            _dbContext = dbContext;

            // Команды с методами без параметров
            AddCommand = new RelayCommand(_ => AddBook());
            SearchCommand = new RelayCommand(_ => PerformSearch());

            // Команды с методами с параметрами
            EditCommand = new RelayCommand(EditBook);
            DeleteCommand = new RelayCommand(DeleteBook);
            PrintCommand = new RelayCommand(PrintBook);

            LoadBooks();
            LoadFilters();
            SelectedFilter = Filters.First();
        }

        private void PrintBook(object parameter)
        {
            if (parameter is Book book)
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    var bookPrintView = new BookPrintView(book);
                    var grid = (Grid)bookPrintView.Content;
                    printDialog.PrintVisual(grid, "Print book invoice");
                }
            }
        }

        // <summary>
        /// Выполняет поиск книг с применением выбранного фильтра и текста поиска.
        /// </summary>
        private void PerformSearch()
        {
            IQueryable<Book> filteredBooks = _dbContext.Books;

            if (!string.IsNullOrWhiteSpace(SelectedFilter))
            {
                if (_filterStrategies.TryGetValue(SelectedFilter, out var strategy))
                {
                    filteredBooks = strategy.Filter(filteredBooks, SearchText);
                }
            }

            LoadBooks(filteredBooks);
        }

        private void LoadBooks(IQueryable<Book> filteredBooks)
        {
            Books = new ObservableCollection<Book>(filteredBooks);
        }

        private void DeleteBook(object parameter)
        {
            if (parameter is Book book)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить книгу из базы данных?",
                    "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (book.Orders.Any())
                    {
                        MessageBox.Show("Книга была закуплена и не может быть удалена!", "Обнаружена закупка книги", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }

                    _dbContext.Books.Remove(book);

                    try
                    {
                        _dbContext.SaveChanges();
                        Books.Remove(book);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Ошибка при удалении", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void EditBook(object parameter)
        {
            if (parameter is Book book)
            {
                BookEditorDataContext = new BookEditorViewModel(_dbContext, book, CloseBookEditor);
                IsBookEditorOpen = true;
            }
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

        private void LoadBooks()
        {
            Books = new ObservableCollection<Book>(_dbContext.Books);
        }

        /// <summary>
        /// Загружает доступные фильтры для поиска
        /// </summary>
        private void LoadFilters()
        {
            _filterStrategies = new Dictionary<string, IFilterStrategy<Book>>
            {
                { "Все", new AllFilterStrategy() },
                { "Название", new BookNameFilterStrategy() },
                { "Автор", new AuthorFilterStrategy() },
                { "Группа", new GroupFilterStrategy() },
                { "Категория", new CategoryFilterStrategy() },
                { "Ключевое слово", new KeywordFilterStrategy() },
                { "Отделение", new OtdelFilterStrategy() },
                { "Описание", new BookDescFilterStrategy() }
            };
        }
    }
}
