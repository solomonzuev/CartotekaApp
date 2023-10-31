using System.Linq;

namespace CartotekaApp.Domain.Interfaces
{
    public interface IFilterStrategy<T>
    {
        IQueryable<T> Filter(IQueryable<T> items, string searchText);
    }
}
