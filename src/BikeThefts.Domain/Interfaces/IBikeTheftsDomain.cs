using BikeThefts.Domain.Entities;
using System.Threading.Tasks;

namespace BikeThefts.Domain.Interfaces
{
    public interface IBikeTheftsDomain
    {
        Task<StolenBikesCount> GetThefts(Filters filter);
    }
}
