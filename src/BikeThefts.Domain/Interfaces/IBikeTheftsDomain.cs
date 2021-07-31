using BikeThefts.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BikeThefts.Domain.Interfaces
{
    public interface IBikeTheftsDomain
    {
        Task<StolenBikes> GetThefts(Filters filter);
        Task<List<StolenBikes>> GetThefts(LocationType locationType);
    }
}
