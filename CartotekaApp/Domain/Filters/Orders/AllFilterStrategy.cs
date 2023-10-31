using CartotekaApp.Domain.Interfaces;
using CartotekaApp.Models;
using System.Linq;

namespace CartotekaApp.Domain.Filters.Orders
{
    public class AllFilterStrategy : IFilterStrategy<Order>
    {
        public IQueryable<Order> Filter(IQueryable<Order> orders, string searchText)
        {
            return orders.Where(o =>
                o.Book.BookName.Contains(searchText) ||
                o.Book.Authors.Any(a => a.FullName.Contains(searchText)) ||
                o.Book.Groups.Any(g => g.GroupName.Contains(searchText)) ||
                o.Book.Category.CategoryName.Contains(searchText) ||
                o.Book.Keywords.Any(k => k.Keyword1.Contains(searchText)) ||
                o.Book.Otdel.Contains(searchText) ||
                o.Book.BookDesc.Contains(searchText) ||
                o.OrderYear.ToString().Contains(searchText) ||
                o.Price.ToString().Contains(searchText) ||
                o.Units.ToString().Contains(searchText));
        }
    }
}
