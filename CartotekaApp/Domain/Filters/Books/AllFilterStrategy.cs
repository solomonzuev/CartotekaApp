using CartotekaApp.Domain.Interfaces;
using CartotekaApp.Models;
using System.Linq;

namespace CartotekaApp.Domain.Filters.Books
{
    public class AllFilterStrategy : IFilterStrategy<Book>
    {
        public IQueryable<Book> Filter(IQueryable<Book> items, string searchText)
        {
            return items.Where(b => b.BookName.Contains(searchText) ||
                b.Otdel.Contains(searchText) ||
                b.Category.CategoryName.Contains(searchText) ||
                b.BookDesc.Contains(searchText) ||
                b.Authors.Any(a => a.FullName.Contains(searchText)) ||
                b.Groups.Any(g => g.GroupName.Contains(searchText)) ||
                b.Keywords.Any(k => k.Keyword1.Contains(searchText)));
        }
    }
}
