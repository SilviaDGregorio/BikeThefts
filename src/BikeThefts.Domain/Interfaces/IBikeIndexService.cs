using BikeThefts.Domain.Entities;
using System.Threading.Tasks;

namespace BikeThefts.Domain.Interfaces
{
    public interface IBikeIndexService
    {
        Task<int> GetThefts(Filters filter);
    }
}
