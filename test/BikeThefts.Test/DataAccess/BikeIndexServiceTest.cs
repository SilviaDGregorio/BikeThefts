using BikeThefts.DataAccess;
using BikeThefts.DataAccess.DTO;
using BikeThefts.DataAccess.Interfaces;
using BikeThefts.Domain.Entities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NSubstitute;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BikeThefts.Test.DataAccess
{
    public class BikeIndexServiceTest
    {
        private readonly ICacheService _cacheService;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<BikeIndexService> _logger;
        private readonly BikeIndexService _bikeService;
        private readonly HttpClient _httpClient;
        public BikeIndexServiceTest()
        {
            _cacheService = Substitute.For<ICacheService>();
            _clientFactory = Substitute.For<IHttpClientFactory>();
            _httpClient = Substitute.For<HttpClient>();
            _clientFactory.CreateClient("bikeindex").Returns(_httpClient);
            _logger = Substitute.For<ILogger<BikeIndexService>>();
            _bikeService = new BikeIndexService(_clientFactory, _logger, _cacheService);

        }

        [Fact]
        public async Task Given_filters_already_on_cache_Return_cache_without_calling()
        {
            //Arrange
            int expected = 1;
            Filters filters = new() { Distance = 10, Location = "Haarlem" };
            _cacheService.GetCache<int?>(filters).Returns(expected);

            //Act
            var result = await _bikeService.GetThefts(filters);

            //Assert
            _clientFactory.DidNotReceive().CreateClient(Arg.Any<string>());
            result.Equals(expected);
        }

        [Fact]
        public async Task Given_filters_not_in_cache_Call_to_get_it()
        {
            //Arrange
            int expected = 5;
            Filters filters = new() { Distance = 10, Location = "Haarlem" };
            BikeTheftsResponse bikeResponse = new BikeTheftsResponse() { Proximity = expected };
            _cacheService.GetCache<int?>(filters).Returns((int?)null);


            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonConvert.SerializeObject(bikeResponse)),
            });

            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
            fakeHttpClient.BaseAddress = new System.Uri("https://bikeindex.org/api/v3/");
            _clientFactory.CreateClient("bikeindex").Returns(fakeHttpClient);

            //Act
            var result = await _bikeService.GetThefts(filters);

            //Assert
            _clientFactory.Received().CreateClient(Arg.Any<string>());
            result.Equals(expected);
        }

        [Fact]
        public async Task Using_bad_parameters_when_calling_the_api_Receive_not_success_status()
        {
            //Arrange

            Filters filters = new() { Distance = -10, Location = "Haarlem" };
            BikeTheftsResponse bikeResponse = new BikeTheftsResponse() { Error = "Error" };
            _cacheService.GetCache<int?>(filters).Returns((int?)null);


            var fakeHttpMessageHandler = new FakeHttpMessageHandler(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent(JsonConvert.SerializeObject(bikeResponse)),
            });

            var fakeHttpClient = new HttpClient(fakeHttpMessageHandler);
            fakeHttpClient.BaseAddress = new System.Uri("https://bikeindex.org/api/v3/");
            _clientFactory.CreateClient("bikeindex").Returns(fakeHttpClient);

            //Act
            var response = await Assert.ThrowsAsync<HttpRequestException>(() => _bikeService.GetThefts(filters));

            //Assert
            response.Message.Equals("Error");
        }

    }
}
