using BikeThefts.DataAccess.DTO;
using BikeThefts.DataAccess.Interfaces;
using BikeThefts.Domain.Entities;
using BikeThefts.Domain.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BikeThefts.DataAccess
{
    public class BikeIndexService : IBikeIndexService
    {
        private readonly ICacheService _cacheService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<BikeIndexService> _logger;


        public BikeIndexService(IHttpClientFactory clientFactory, ILogger<BikeIndexService> logger, ICacheService cacheService)
        {
            _clientFactory = clientFactory;
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task<int> GetThefts(Filters filters)
        {
            try
            {
                var cache = _cacheService.GetCache<int?>(filters);
                if (cache != null) return cache.Value;

                var client = _clientFactory.CreateClient("bikeindex");
                var queryParams = new Dictionary<string, string>()
                {
                    {"location", filters.Location },
                    {"distance", filters.Distance.ToString() }
                };
                string url = QueryHelpers.AddQueryString("search/count", queryParams);
                var response = await client.GetAsync(url);
                var content = JsonConvert.DeserializeObject<BikeTheftsResponse>(await response.Content.ReadAsStringAsync());

                if (response.IsSuccessStatusCode)
                {
                    _cacheService.SetCache(filters, content.Proximity);
                    return content.Proximity;
                }
                else
                {
                    throw new HttpRequestException(content.Error, null, response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                string message = $"Something went wrong trying to GetThefts for Location: {filters.Location}, Distance: {filters.Distance}";
                _logger.LogError(message, ex);
                throw;
            }

        }
    }
}
