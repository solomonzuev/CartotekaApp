using CartotekaApp.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CartotekaApp.Domain
{
    public class AuthorsWindowViewModel : ObservableObject
    {
        private readonly ICartotekaDbContext _dbContext;
        private Author _selectedAuthorToAdd;
        private Author _selectedAuthorToRemove;

        public Author SelectedAuthorToRemove
        {
            get => _selectedAuthorToRemove;
            set => SetProperty(ref _selectedAuthorToRemove, value);
        }
        public Author SelectedAuthorToAdd
        {
            get => _selectedAuthorToAdd;
            set => SetProperty(ref _selectedAuthorToAdd, value);
        }
        public ObservableCollection<Author> Authors { get; }
        public ObservableCollection<Author> SelectedAuthors { get; }
        public Book CurrentBook { get; }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public AuthorsWindowViewModel(ICartotekaDbContext dbContext, Book book)
        {
            _dbContext = dbContext;
            CurrentBook = book;

            AddCommand = new RelayCommand(_ => AddBookAuthor());
            DeleteCommand = new RelayCommand(_ => DeleteBookAuthor());

            Authors = new ObservableCollection<Author>(_dbContext.Authors);
            SelectedAuthors = new ObservableCollection<Author>();
            FillSelectedAuthors();
        }

        private void DeleteBookAuthor()
        {
            if (SelectedAuthorToRemove != null)
            {
                CurrentBook.Authors.Remove(SelectedAuthorToRemove);
                Authors.Add(SelectedAuthorToRemove);
                SelectedAuthors.Remove(SelectedAuthorToRemove); // После удаления в этой строке сбрасывается SelectedAuthorToRemove
            }
        }

        private void AddBookAuthor()
        {
            if (SelectedAuthorToAdd != null)
            {
                CurrentBook.Authors.Add(SelectedAuthorToAdd);
                SelectedAuthors.Add(SelectedAuthorToAdd);
                Authors.Remove(SelectedAuthorToAdd); // После удаления в этой строке сбрасывается SelectedAuthorToAdd
            }
        }

        private void FillSelectedAuthors()
        {
            foreach (var author in CurrentBook.Authors)
            {
                var selectedAuthor = Authors.FirstOrDefault(a => a.Id == author.Id);
                if (selectedAuthor != null)
                {
                    SelectedAuthors.Add(selectedAuthor);
                    Authors.Remove(selectedAuthor);
                }
            }
        }
    }
}
