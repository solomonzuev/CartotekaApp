using CartotekaApp.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CartotekaApp.Domain
{
    public class BookEditorViewModel : ObservableObject
    {
        private readonly ICartotekaDbContext _dbContext;
        private Category _category;
        private string _errorMessage;

        public Book CurrentBook { get; }
        public ObservableCollection<Category> Categories { get; set; }
        public Category SelectedCategory
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand SaveCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand RemoveCommand { get; }
        public ICommand AuthorsCommand { get; }
        public ICommand GroupsCommand { get; }
        public ICommand KeywordsCommand { get; }

        public BookEditorViewModel(ICartotekaDbContext dbContext, Book book, Action closeMethod)
        {
            _dbContext = dbContext;
            CurrentBook = book;

            SaveCommand = new RelayCommand(_ => SaveBook());
            CloseCommand = new RelayCommand(_ => closeMethod());
            AuthorsCommand = new RelayCommand(_ => OpenAuthorsEditor());
            GroupsCommand = new RelayCommand(_ => OpenGroupsEditor());
            KeywordsCommand = new RelayCommand(_ => OpenKeywordsEditor());

            Categories = new ObservableCollection<Category>(_dbContext.Categories);
            SelectedCategory = Categories.FirstOrDefault(c => c.Id == CurrentBook.CategoryId);
        }

        private void OpenKeywordsEditor()
        {
            var wnd = new KeywordsWindow()
            {
                DataContext = new KeywordsWindowViewModel(_dbContext, CurrentBook),
            };
            wnd.ShowDialog();
        }

        private void OpenGroupsEditor()
        {
            var wnd = new GroupsWindow()
            {
                DataContext = new GroupsWindowViewModel(_dbContext, CurrentBook),
            };
            wnd.ShowDialog();
        }

        private void OpenAuthorsEditor()
        {
            var wnd = new AuthorsWindow()
            {
                DataContext = new AuthorsWindowViewModel(_dbContext, CurrentBook),
            };
            wnd.ShowDialog();
        }

        private void SaveBook()
        {
            ValidateBook();

            if (!string.IsNullOrWhiteSpace(ErrorMessage))
            {
                return;
            }

            CurrentBook.Category = SelectedCategory;

            if (CurrentBook.Id == 0)
            {
                _dbContext.Books.Add(CurrentBook);
            }

            try
            {
                _dbContext.SaveChanges();
                CloseCommand.Execute(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка при сохранении", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ValidateBook()
        {
            if (string.IsNullOrWhiteSpace(CurrentBook.BookName))
            {
                ErrorMessage = "Название книги не может быть пустым!";
            }
            else if (string.IsNullOrWhiteSpace(CurrentBook.Otdel))
            {
                ErrorMessage = "Отделение должно быть заполнено!";
            }
            else if (SelectedCategory == null)
            {
                ErrorMessage = "Не выбрана категория, к которой относится книга!";
            }
            else
            {
                ErrorMessage = string.Empty;
            }
        }
    }
}
