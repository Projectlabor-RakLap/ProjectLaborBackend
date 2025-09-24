using AutoMapper;
using ProjectLaborBackend.Dtos.StockChange;
using ProjectLaborBackend.Entities;
namespace ProjectLaborBackend.Profiles
{
    public class StockChangeProfile : Profile
    {
        public StockChangeProfile()
        { 
            CreateMap<StockChange, StockChangeGetDTO>();
            CreateMap<StockChangeCreateDTO, StockChange>();
            CreateMap<StockChangeUpdateDTO, StockChange>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
