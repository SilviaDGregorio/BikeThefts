using BikeThefts.Domain;
using BikeThefts.Domain.Entities;
using BikeThefts.Domain.Interfaces;
using BikeThefts.Domain.Settings;
using FluentAssertions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace BikeThefts.Test.Domain
{
    public class BikeTheftsDomainTest
    {
        private readonly IBikeIndexService _bikeService;
        private readonly IOptions<Locations> _settings;
        private readonly BikeTheftsDomain _bikeTheftsDomain;
        private readonly Filters _operativeFilteres;
        private readonly Filters _expandFilteres;

        public BikeTheftsDomainTest()
        {
            _bikeService = Substitute.For<IBikeIndexService>();
            _operativeFilteres = new() { Distance = 10, Location = "Haarlem" };
            _expandFilteres = new() { Distance = 5, Location = "Elche" };
            _settings = Options.Create<Locations>(new Locations()
            {
                Expand = new List<Filters>() { _expandFilteres },
                Operative = new List<Filters>() { _operativeFilteres }
            });

            _bikeTheftsDomain = new BikeTheftsDomain(_bikeService, _settings);
            _bikeService.GetThefts(_operativeFilteres).Returns(100);
            _bikeService.GetThefts(_expandFilteres).Returns(1);
        }

        [Fact]
        public async Task Given_filters_Stolen_bikes_are_return()
        {
            //Arrange
            StolenBikes expected = new() { Location = "Haarlem", Distance = 10, Thefts = 100 };
            _bikeService.GetThefts(_operativeFilteres).Returns(100);

            //Act
            var response = await _bikeTheftsDomain.GetThefts(_operativeFilteres);

            //Assert
            response.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Given_locationType_operative_Operative_bikes_are_return()
        {
            //Arrange
            List<StolenBikes> expected = new()
            {
                new StolenBikes { Location = "Haarlem", Distance = 10, Thefts = 100 }
            };

            LocationType locationType = LocationType.Operative;

            //Act
            var response = await _bikeTheftsDomain.GetThefts(locationType);

            //Assert
            response.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Given_locationType_expand_Operative_bikes_are_return()
        {
            //Arrange
            List<StolenBikes> expected = new()
            {
                new StolenBikes { Location = "Elche", Distance = 5, Thefts = 1 }
            };

            LocationType locationType = LocationType.Expand;

            //Act
            var response = await _bikeTheftsDomain.GetThefts(locationType);

            //Assert
            response.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task Given_locationType_not_valid_Exception_argument_not_valid()
        {
            //Act
            var response = await Assert.ThrowsAsync<ArgumentException>(() => _bikeTheftsDomain.GetThefts((LocationType)5));
            //Assert
            response.Message.Should().Be("LocationType is not valid");
        }
    }
}
