using AutoMapper;
using ProductService.Domain;
using SharedKernel.Dtos;

namespace ProductService.Application.Mappers
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductDto, Product>();
        }
    }
}
