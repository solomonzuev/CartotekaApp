using CartotekaApp.Domain.Interfaces;
using CartotekaApp.Models;
using System.Linq;

namespace CartotekaApp.Domain.Filters.Orders
{
    public class PriceFilterStrategy : IFilterStrategy<Order>
    {
        public IQueryable<Order> Filter(IQueryable<Order> orders, string searchText)
        {
            return orders.Where(o => o.Price.ToString().Contains(searchText));
        }
    }
}
