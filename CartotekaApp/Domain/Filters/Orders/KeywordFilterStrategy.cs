using CartotekaApp.Domain.Interfaces;
using CartotekaApp.Models;
using System.Linq;

namespace CartotekaApp.Domain.Filters.Orders
{
    public class KeywordFilterStrategy : IFilterStrategy<Order>
    {
        public IQueryable<Order> Filter(IQueryable<Order> orders, string searchText)
        {
            return orders.Where(o => o.Book.Keywords.Any(k => k.Keyword1.Contains(searchText)));
        }
    }
}
