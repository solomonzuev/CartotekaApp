using CartotekaApp.Domain.Interfaces;
using CartotekaApp.Models;
using System.Linq;

namespace CartotekaApp.Domain.Filters.Orders
{
    public class OrderYearFilterStrategy : IFilterStrategy<Order>
    {
        public IQueryable<Order> Filter(IQueryable<Order> orders, string searchText)
        {
            return orders.Where(o => o.OrderYear.ToString().Contains(searchText));
        }
    }
}
