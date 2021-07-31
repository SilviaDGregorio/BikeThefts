using BikeThefts.Domain.Entities;
using System.Threading.Tasks;

namespace BikeThefts.Domain.Interfaces
{
    public interface IBikeIndexService
    {
        Task<StolenBikes> GetThefts(Filters filter);
    }
}
