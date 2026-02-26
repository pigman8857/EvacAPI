using AutoMapper;
using evacPlanMoni.entities;
using evacPlanMoni.presentations.dtos;

namespace evacPlanMoni.infras.mappers;

public class EvacuationProfile : Profile
{
  public EvacuationProfile()
  {
    CreateMap<AddEvacuationZoneDto, EvacuationZone>();
    CreateMap<AddVehicleDto, Vehicle>();
    // // You can also map from Entity to DTO
    // CreateMap<ProductEntity, ProductDto>();
  }
}
