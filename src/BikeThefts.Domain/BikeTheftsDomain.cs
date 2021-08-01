using BikeThefts.Domain.Entities;
using BikeThefts.Domain.Interfaces;
using BikeThefts.Domain.Settings;
using Microsoft.Extensions.Options;
using System;
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
            var count = await _bikeService.GetThefts(filter);
            return new StolenBikes() { Location = filter.Location, Thefts = count, Distance = filter.Distance };
        }

        public async Task<List<StolenBikes>> GetThefts(LocationType locationType)
        {
            return locationType switch
            {
                LocationType.Operative => await GetTheftsPerLocationsType(_locations.Operative),
                LocationType.Expand => await GetTheftsPerLocationsType(_locations.Expand),
                _ => throw new ArgumentException("LocationType is not valid")
            };
        }

        private async Task<List<StolenBikes>> GetTheftsPerLocationsType(List<Filters> locations)
        {
            List<Task<StolenBikes>> listOfTasks = new();
            List<StolenBikes> bikesList = new();
            if (locations != null)
            {
                foreach (var location in locations)
                {
                    listOfTasks.Add(GetThefts(location));
                }
                await Task.WhenAll(listOfTasks);
                foreach (var task in listOfTasks)
                {
                    bikesList.Add(task.Result);
                }
            }
            return bikesList;
        }
    }
}
