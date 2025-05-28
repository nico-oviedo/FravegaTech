using AutoMapper;
using OrderService.Domain;
using OrderService.Domain.Enums;
using SharedKernel.Dtos;

namespace OrderService.Application.Mappers
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EventId))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<EventDto, Event>()
                .ForMember(dest => dest.EventId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<OrderStatus>(src.Type)));
        }
    }
}
