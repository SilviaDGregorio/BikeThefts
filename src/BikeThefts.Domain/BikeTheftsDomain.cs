using BikeThefts.Domain.Entities;
using BikeThefts.Domain.Interfaces;
using System.Threading.Tasks;

namespace BikeThefts.Domain
{
    public class BikeTheftsDomain : IBikeTheftsDomain
    {
        private readonly IBikeIndexService _bikeService;
        public BikeTheftsDomain(IBikeIndexService bikeService)
        {
            _bikeService = bikeService;
        }

        public async Task<StolenBikesCount> GetThefts(Filters filter)
        {
            return await _bikeService.GetThefts(filter);
        }
    }
}
