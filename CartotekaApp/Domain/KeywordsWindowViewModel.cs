using CartotekaApp.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CartotekaApp.Domain
{
    public class KeywordsWindowViewModel : ObservableObject
    {
        private readonly ICartotekaDbContext _dbContext;
        private Keyword _selectedKeywordToAdd;
        private Keyword _selectedKeywordToRemove;

        public Keyword SelectedKeywordToRemove
        {
            get => _selectedKeywordToRemove;
            set => SetProperty(ref _selectedKeywordToRemove, value);
        }
        public Keyword SelectedKeywordToAdd
        {
            get => _selectedKeywordToAdd;
            set => SetProperty(ref _selectedKeywordToAdd, value);
        }
        public ObservableCollection<Keyword> Keywords { get; }
        public ObservableCollection<Keyword> SelectedKeywords { get; }
        public Book CurrentBook { get; }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public KeywordsWindowViewModel(ICartotekaDbContext dbContext, Book book)
        {
            _dbContext = dbContext;
            CurrentBook = book;

            AddCommand = new RelayCommand(_ => AddBookKeyword());
            DeleteCommand = new RelayCommand(_ => DeleteBookKeyword());

            Keywords = new ObservableCollection<Keyword>(_dbContext.Keywords);
            SelectedKeywords = new ObservableCollection<Keyword>();
            FillSelectedKeywords();
        }

        private void DeleteBookKeyword()
        {
            if (SelectedKeywordToRemove != null)
            {
                CurrentBook.Keywords.Remove(SelectedKeywordToRemove);
                Keywords.Add(SelectedKeywordToRemove);
                SelectedKeywords.Remove(SelectedKeywordToRemove); // После удаления в этой строке сбрасывается SelectedKeywordToRemove
            }
        }

        private void AddBookKeyword()
        {
            if (SelectedKeywordToAdd != null)
            {
                CurrentBook.Keywords.Add(SelectedKeywordToAdd);
                SelectedKeywords.Add(SelectedKeywordToAdd);
                Keywords.Remove(SelectedKeywordToAdd); // После удаления в этой строке сбрасывается SelectedKeywordToAdd
            }
        }

        private void FillSelectedKeywords()
        {
            foreach (var Keyword in CurrentBook.Keywords)
            {
                var selectedKeyword = Keywords.FirstOrDefault(k => k.Id == Keyword.Id);
                if (selectedKeyword != null)
                {
                    SelectedKeywords.Add(selectedKeyword);
                    Keywords.Remove(selectedKeyword);
                }
            }
        }
    }
}
