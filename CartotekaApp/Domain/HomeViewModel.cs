using CartotekaApp.Domain.Filters.Orders;
using CartotekaApp.Domain.Interfaces;
using CartotekaApp.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CartotekaApp.Domain
{
    /// <summary>
    /// ViewModel для домашней страницы приложения.
    /// </summary>
    public class HomeViewModel : ObservableObject
    {
        private readonly ICartotekaDbContext _dbContext;
        private ObservableCollection<Order> _orders;
        private string _selectedFilter;
        private string _searchText;
        private Dictionary<string, IFilterStrategy<Order>> _filterStrategies;
        private bool _isOrderEditorDialogOpen;
        private object _orderEditorDataContext;
        private Visibility _isEditButtonVisibility;

        public Visibility IsEditButtonVisibility
        {
            get => _isEditButtonVisibility;
            set => SetProperty(ref _isEditButtonVisibility, value);
        }

        /// <summary>
        /// Контекст данных для окна добавления/редактирования закупок
        /// </summary>
        public object OrderEditorDataContext
        {
            get => _orderEditorDataContext;
            set => SetProperty(ref _orderEditorDataContext, value);
        }

        /// <summary>
        /// Определяет открыто или скрыто диалоговое окно для 
        /// добавления/редактирования закупок
        /// </summary>
        public bool IsOrderEditorDialogOpen
        {
            get => _isOrderEditorDialogOpen;
            set => SetProperty(ref _isOrderEditorDialogOpen, value);
        }

        /// <summary>
        /// Доступные фильтры для поиска
        /// </summary>
        public IEnumerable<string> Filters => _filterStrategies.Keys;

        /// <summary>
        /// Коллекция закупок
        /// </summary>
        public ObservableCollection<Order> Orders
        {
            get => _orders;
            set => SetProperty(ref _orders, value);
        }

        /// <summary>
        /// Текст для поиска
        /// </summary>
        public string SearchText
        {
            get => _searchText ?? string.Empty;
            set => SetProperty(ref _searchText, value);
        }

        /// <summary>
        /// Выбранный фильтр для поиска
        /// </summary>
        public string SelectedFilter
        {
            get => _selectedFilter;
            set => SetProperty(ref _selectedFilter, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand WordExportCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand PrintBookCommand { get; }

        public HomeViewModel(ICartotekaDbContext dbContext, User currentUser)
        {
            _dbContext = dbContext;

            // Определяет, нужно ли отображать кнопку для редактирования данных
            IsEditButtonVisibility = currentUser.Role.RoleName == "Администратор"
                ? Visibility.Visible : Visibility.Collapsed;

            // Связка методов без параметров с командами
            SearchCommand = new RelayCommand(_ => PerformSearch());
            AddCommand = new RelayCommand(_ => AddOrder());
            WordExportCommand = new RelayCommand(_ => ExportOrdersToWord());

            // Связка методов с параметрами с командами
            DeleteCommand = new RelayCommand(DeleteOrder);
            EditCommand = new RelayCommand(EditOrder);
            PrintCommand = new RelayCommand(PrintOrders);
            PrintBookCommand = new RelayCommand(PrintBook);

            LoadOrders(); // Загрузка закупок
            LoadFilters(); // Загрузка фильтров для поиска
            SelectedFilter = Filters.First();
        }

        private void ExportOrdersToWord()
        {
            var fileName = GetFilePathFromUser();

            if (!string.IsNullOrEmpty(fileName))
            {
                TryGenerateAndOpenReport(fileName);
            }
        }

        private void TryGenerateAndOpenReport(string fileName)
        {
            try
            {
                GenerateReport(fileName);
                OpenWord(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void OpenWord(string fileName)
        {
            Process.Start(fileName);
        }

        private void GenerateReport(string pathToSave)
        {
            var reportGenerator = new WordReportGenerator(_orders, pathToSave);
            reportGenerator.GenerateReport();
        }

        private string GetFilePathFromUser()
        {
            var sf = new SaveFileDialog()
            {
                Filter = "Word Document|*.docx",
                Title = "Выберите место для сохранения файла",
            };

            sf.ShowDialog();

            return sf.FileName;
        }

        private void PrintBook(object parameter)
        {
            if (parameter is Order order)
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    var bookPrintView = new BookPrintView(order.Book);
                    var grid = (Visual)bookPrintView.Content;
                    printDialog.PrintVisual(grid, "Данные о книге");
                }
            }
        }

        private void PrintOrders(object parameter)
        {
            if (parameter is DataGrid)
            {
                try
                {
                    var printerAdapter = new DataGridViewPrinterAdapter(Orders);
                    printerAdapter.Print();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при попытке печати!", ex.ToString());
                }
            }
        }

        private void EditOrder(object parameter)
        {
            if (parameter is Order order)
            {
                OrderEditorDataContext = new OrderEditorViewModel(_dbContext, order, CloseOrderEditor);
                IsOrderEditorDialogOpen = true;
            }
        }

        /// <summary>
        /// Закрывает модальное окно
        /// </summary>
        private void CloseOrderEditor()
        {
            IsOrderEditorDialogOpen = false;
            OrderEditorDataContext = null;
            LoadOrders();
        }

        /// <summary>
        /// Выполняет поиск закупок с применением выбранного фильтра и текста поиска.
        /// </summary>
        private void PerformSearch()
        {
            IQueryable<Order> filteredOrders = _dbContext.Orders;

            if (!string.IsNullOrWhiteSpace(SelectedFilter))
            {
                if (_filterStrategies.TryGetValue(SelectedFilter, out var strategy))
                {
                    filteredOrders = strategy.Filter(filteredOrders, SearchText);
                }
            }

            Orders = new ObservableCollection<Order>(filteredOrders);
        }

        /// <summary>
        /// Удаляет выбранную закупку.
        /// </summary>
        private void DeleteOrder(object parameter)
        {
            if (MessageBox.Show("Вы уверены, что хотите удалить запись о закупке?",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var order = (Order)parameter;

                try
                {
                    _dbContext.Orders.Remove(order);
                    _dbContext.SaveChanges();

                    Orders.Remove(order);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Ошибка при удалении", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// Открывает окно для добавления новой закупки.
        /// </summary>
        private void AddOrder()
        {
            OrderEditorDataContext = new OrderEditorViewModel(_dbContext, new Order(), CloseOrderEditor);
            IsOrderEditorDialogOpen = true;
        }

        /// <summary>
        /// Загружает закупки из базы данных.
        /// </summary>
        private void LoadOrders()
        {
            Orders = new ObservableCollection<Order>(_dbContext.Orders);
        }

        /// <summary>
        /// Загружает доступные фильтры для поиска
        /// </summary>
        private void LoadFilters()
        {
            _filterStrategies = new Dictionary<string, IFilterStrategy<Order>>
            {
                { "Все", new AllFilterStrategy() },
                { "Название", new BookNameFilterStrategy() },
                { "Автор", new AuthorFilterStrategy() },
                { "Группа", new GroupFilterStrategy() },
                { "Категория", new CategoryFilterStrategy() },
                { "Ключевое слово", new KeywordFilterStrategy() },
                { "Отделение", new OtdelFilterStrategy() },
                { "Описание", new BookDescFilterStrategy() },
                { "Год закупки", new OrderYearFilterStrategy() },
                { "Цена", new PriceFilterStrategy() },
                { "Количество", new UnitsFilterStrategy() },
            };
        }
    }
}
