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
            CreateMap<StockCreateDTO, Stock>();
            CreateMap<StockUpdateDto, Stock>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
