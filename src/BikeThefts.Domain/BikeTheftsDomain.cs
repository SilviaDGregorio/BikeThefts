using BikeThefts.Domain.Entities;
using BikeThefts.Domain.Interfaces;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BikeThefts.Domain
{
    public class BikeTheftsDomain : IBikeTheftsDomain
    {
        private readonly IBikeIndexService _bikeService;
        private readonly Locations _locations;
        public BikeTheftsDomain(IBikeIndexService bikeService, IOptions<Locations> settings)
        {
            _bikeService = bikeService;
            _locations = settings.Value;
        }

        public async Task<StolenBikes> GetThefts(Filters filter)
        {
            return await _bikeService.GetThefts(filter);
        }

        public async Task<List<StolenBikes>> GetThefts(LocationType locationType)
        {
            return locationType switch
            {

                LocationType.Operative => await GetTheftsPerLocationsType(_locations.Operative),
                LocationType.Expand => await GetTheftsPerLocationsType(_locations.Expand),
                _ => throw new System.Exception("LocationType is not valid")
            };
        }

        private async Task<List<StolenBikes>> GetTheftsPerLocationsType(List<Filters> locations)
        {
            List<StolenBikes> bikesList = new List<StolenBikes>();
            if (locations != null)
            {
                foreach (var location in locations)
                {
                    bikesList.Add(await GetThefts(location));
                }
            }
            return bikesList;
        }


    }
}
