using AutoMapper;
using evacPlanMoni.entities;
using evacPlanMoni.presentations.dtos;

namespace evacPlanMoni.infras.mappers;

public class EvacuationProfile : Profile
{
  public EvacuationProfile()
  {
    CreateMap<AddEvacuationZoneDto, EvacuationZone>()
      .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.LocationCoordinates.latitude))
      .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.LocationCoordinates.longitude));
    CreateMap<AddVehicleDto, Vehicle>()
      .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.LocationCoordinates.latitude))
      .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.LocationCoordinates.longitude));
    // // You can also map from Entity to DTO
    // CreateMap<ProductEntity, ProductDto>();
  }
}
