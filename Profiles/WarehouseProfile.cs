using AutoMapper;
using ProjectLaborBackend.Dtos.Warehouse;
using ProjectLaborBackend.Entities;
namespace ProjectLaborBackend.Profiles
{
    public class WarehouseProfile : Profile
    {
        public WarehouseProfile()
        {
            CreateMap<Warehouse, WarehouseGetDTO>();
            CreateMap<WarehousePostDTO, Warehouse>();
            CreateMap<WarehouseUpdateDTO, Warehouse>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); 
        }
    }
}
