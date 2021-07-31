using AutoMapper;
using BikeThefts.Api.DTO;

namespace BikeThefts.Api.Mapper
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<Filters, Domain.Entities.Filters>();
        }
    }
}
