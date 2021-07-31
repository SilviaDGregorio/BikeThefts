using AutoMapper;
using BikeThefts.Api.DTO;
using BikeThefts.Domain.Entities;

namespace BikeThefts.Api.Mapper
{
    public class OrganizationProfile : Profile
    {
        public OrganizationProfile()
        {
            CreateMap<DTO.Filters, Domain.Entities.Filters>();
            CreateMap<StolenBikes, BikeTheftsReturn>()
                .ForMember(dest => dest.Thefts, opt => opt.MapFrom(x => x.Thefts));
        }
    }
}
