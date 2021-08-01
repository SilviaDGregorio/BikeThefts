using BikeThefts.Domain.Entities;

namespace BikeThefts.DataAccess.Interfaces
{
    public interface ICacheService
    {
        T GetCache<T>(Filters key);
        void SetCache<T>(Filters key, T cacheEntry);
    }
}
