using AutoMapper;
using Library.ControllerApi.DTOs;
using Library.Data.Entities;

namespace Library.ControllerApi.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Being explicit
        CreateMap<InventoryItem, InventoryDTO>()
            .ForCtorParam("Sku", o => o.MapFrom(s => s.Product.Sku))
            .ForCtorParam("Name", o => o.MapFrom(s => s.Product.Name))
            .ForCtorParam("CurrentStock", o => o.MapFrom(s => s.CurrentStock))
            .ForCtorParam("Price", o => o.MapFrom(s => s.Product.Price))
            .ReverseMap();

        // Make it implicit
        // CreateMap<InventoryItem, InventoryDTO>();
    }
}