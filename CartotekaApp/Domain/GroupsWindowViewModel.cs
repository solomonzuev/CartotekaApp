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
    public class GroupsWindowViewModel : ObservableObject
    {
        private readonly ICartotekaDbContext _dbContext;
        private Group _selectedGroupToAdd;
        private Group _selectedGroupToRemove;

        public Group SelectedGroupToRemove
        {
            get => _selectedGroupToRemove;
            set => SetProperty(ref _selectedGroupToRemove, value);
        }
        public Group SelectedGroupToAdd
        {
            get => _selectedGroupToAdd;
            set => SetProperty(ref _selectedGroupToAdd, value);
        }
        public ObservableCollection<Group> Groups { get; }
        public ObservableCollection<Group> SelectedGroups { get; }
        public Book CurrentBook { get; }

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public GroupsWindowViewModel(ICartotekaDbContext dbContext, Book book)
        {
            _dbContext = dbContext;
            CurrentBook = book;

            AddCommand = new RelayCommand(_ => AddBookGroup());
            DeleteCommand = new RelayCommand(_ => DeleteBookGroup());

            Groups = new ObservableCollection<Group>(_dbContext.Groups);
            SelectedGroups = new ObservableCollection<Group>();
            FillSelectedGroups();
        }

        private void DeleteBookGroup()
        {
            if (SelectedGroupToRemove != null)
            {
                CurrentBook.Groups.Remove(SelectedGroupToRemove);
                Groups.Add(SelectedGroupToRemove);
                SelectedGroups.Remove(SelectedGroupToRemove); // После удаления в этой строке сбрасывается SelectedGroupToRemove
            }
        }

        private void AddBookGroup()
        {
            if (SelectedGroupToAdd != null)
            {
                CurrentBook.Groups.Add(SelectedGroupToAdd);
                SelectedGroups.Add(SelectedGroupToAdd);
                Groups.Remove(SelectedGroupToAdd); // После удаления в этой строке сбрасывается SelectedGroupToAdd
            }
        }

        private void FillSelectedGroups()
        {
            foreach (var Group in CurrentBook.Groups)
            {
                var selectedGroup = Groups.FirstOrDefault(g => g.Id == Group.Id);
                if (selectedGroup != null)
                {
                    SelectedGroups.Add(selectedGroup);
                    Groups.Remove(selectedGroup);
                }
            }
        }
    }
}
