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
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<BikeIndexService> _logger;
        public BikeIndexService(IHttpClientFactory clientFactory, ILogger<BikeIndexService> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<StolenBikes> GetThefts(Filters filters)
        {
            try
            {

                var client = _clientFactory.CreateClient("bikeindex");
                var queryParams = new Dictionary<string, string>()
                {
                    {"location", filters.Location },
                    {"distance", filters.Distance.ToString() }
                };
                string url = QueryHelpers.AddQueryString("search/count", queryParams);
                var response = await client.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    StolenBikes stolenBikes = new() { Distance = filters.Distance, Location = filters.Location };
                    stolenBikes.Thefts = JsonConvert.DeserializeObject<StolenBikes>(content).Thefts;
                    return stolenBikes;

                }
                else
                {
                    var error = JsonConvert.DeserializeObject<ResponseError>(content);
                    throw new HttpRequestException(error.Error, null, response.StatusCode);
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
