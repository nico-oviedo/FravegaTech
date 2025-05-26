using AutoMapper;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Dtos;
using SharedKernel.Dtos.Requests;
using SharedKernel.Dtos.Responses;

namespace OrderService.Application.Mappers
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Channel, opt => opt.MapFrom(src => src.Channel.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<OrderRequestDto, Order>()
                .ForMember(dest => dest.Channel, opt => opt.MapFrom(src => Enum.GetName(typeof(SourceChannel), src.Channel)));

            CreateMap<Order, OrderTranslatedDto>();
            // Hay que ver como mapear los dos campos a traducir

        }
    }
}
