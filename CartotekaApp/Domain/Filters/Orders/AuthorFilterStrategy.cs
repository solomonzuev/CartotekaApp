using CartotekaApp.Domain.Interfaces;
using CartotekaApp.Models;
using System.Linq;

namespace CartotekaApp.Domain.Filters.Orders
{
    public class AuthorFilterStrategy : IFilterStrategy<Order>
    {
        public IQueryable<Order> Filter(IQueryable<Order> orders, string searchText)
        {
            return orders.Where(o => o.Book.Authors.Any(a => a.FullName.Contains(searchText)));
        }
    }
}
