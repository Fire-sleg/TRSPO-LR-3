using AutoMapper;
using LR_3.Models;
using LR_3.Models.Dto;
using Microsoft.Extensions.Logging;

namespace LR_3
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Recipe, RecipeDTO>().ReverseMap(); ;

            CreateMap<Recipe, RecipeCreateDTO>().ReverseMap();
            CreateMap<Recipe, RecipeUpdateDTO>().ReverseMap();


            CreateMap<Product, ProductDTO>().ReverseMap(); ;

            CreateMap<Product, ProductCreateDTO>().ReverseMap();
            CreateMap<Product, ProductUpdateDTO>().ReverseMap();

            CreateMap<RegistrationRequestDTO, LocalUser>();
        }

    }
}
