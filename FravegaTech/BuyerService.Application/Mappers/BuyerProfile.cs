using AutoMapper;
using BuyerService.Domain;
using SharedKernel.Dtos;

namespace BuyerService.Application.Mappers
{
    public class BuyerProfile : Profile
    {
        public BuyerProfile()
        {
            CreateMap<Buyer, BuyerDto>();
            CreateMap<BuyerDto, Buyer>();
        }
    }
}
