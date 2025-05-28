using AutoMapper;
using OrderService.Domain;
using OrderService.Domain.Enums;
using OrderService.Domain.Enums.Translations;
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
                .ForMember(dest => dest.Channel, opt => opt.MapFrom(src => Enum.Parse<SourceChannel>(src.Channel)))
                .ForMember(dest => dest.Products, opt => opt.Ignore());

            CreateMap<Order, OrderCreatedDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.UpdatedOn, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<Order, OrderTranslatedDto>()
                .ForMember(dest => dest.Products, opt => opt.Ignore())
                .ForMember(dest => dest.Channel, opt => opt.MapFrom(src => src.Channel.ToString()))
                .ForMember(dest => dest.ChannelTranslate, opt => opt.MapFrom(src => SourceChannel_es.Translations.GetValueOrDefault(src.Channel)))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StatusTranslate, opt => opt.MapFrom(src => OrderStatus_es.Translations.GetValueOrDefault(src.Status)));
        }
    }
}
