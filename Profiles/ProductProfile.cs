using AutoMapper;
using ProjectLaborBackend.Dtos.Product;
using ProjectLaborBackend.Entities;

namespace ProjectLaborBackend.Profiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile() 
        {
            CreateMap<Product, ProductGetDTO>();        
            CreateMap<ProductCreateDTO, Product>();
            CreateMap<ProductUpdateDTO, Product>()
                .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
