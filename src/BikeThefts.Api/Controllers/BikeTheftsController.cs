using AutoMapper;
using BikeThefts.Domain.Entities;
using BikeThefts.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BikeThefts.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BikeTheftsController : ControllerBase
    {
        private readonly IBikeTheftsDomain _bikeTheftsDomain;
        private readonly IMapper _mapper;

        public BikeTheftsController(IBikeTheftsDomain bikeTheftsDomain, IMapper mapper)
        {
            _bikeTheftsDomain = bikeTheftsDomain;
            _mapper = mapper;
        }

        /// <summary>
        /// Get number of stolen bikes
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     Get /BikeThefts
        ///     {
        ///        "location": "Amsterdam"// Examples: “210 NW 11th Ave, Portland, OR”, “60647”, “Chicago, IL”, or “45.521728,-122.67326”,
        ///        "distance": 1 // Distance in km
        ///     }
        ///
        /// </remarks>
        /// <param name="filters">Location and distance to search the thefts </param>
        /// <returns>Number of stolen bikes</returns>
        /// <response code="200">Number of stolen bikes</response>
        /// <response code="400">Filters are not correct</response>   
        /// <response code="500">Something went wrong</response>   

        [HttpGet()]
        public async Task<StolenBikesCount> Get([FromQuery] DTO.Filters filters)
        {
            return await _bikeTheftsDomain.GetThefts(_mapper.Map<Domain.Entities.Filters>(filters));
        }
    }
}
