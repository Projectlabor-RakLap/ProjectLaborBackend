using AutoMapper;
using ProjectLaborBackend.Dtos.Stock;
using ProjectLaborBackend.Entities;

namespace ProjectLaborBackend.Profiles
{
    public class StockProfile : Profile
    {
        public StockProfile()
        {
            CreateMap<Stock, StockGetDTO>();
            CreateMap<Stock, StockGetWithProductDTO>();
            CreateMap<StockCreateDTO, Stock>();
            CreateMap<StockUpdateDto, Stock>()
                .ForMember(x => x.WarehouseId, opt => opt.Ignore())
                .ForMember(x => x.ProductId, opt => opt.Ignore())
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
