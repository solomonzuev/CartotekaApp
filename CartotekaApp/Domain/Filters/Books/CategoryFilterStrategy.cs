using CartotekaApp.Domain.Interfaces;
using CartotekaApp.Models;
using System.Linq;

namespace CartotekaApp.Domain.Filters.Books
{
    public class CategoryFilterStrategy : IFilterStrategy<Book>
    {
        public IQueryable<Book> Filter(IQueryable<Book> items, string searchText)
        {
            return items.Where(b => b.Category.CategoryName.Contains(searchText));
        }
    }
}
