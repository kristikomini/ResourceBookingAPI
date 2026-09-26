using AutoMapper;
using ResourceBooking.Dto;
using ResourceBooking.Dtos;
using ResourceBooking.Models;

namespace ResourceBooking.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserForCreationDto, User>();
            CreateMap<UserForUpdateDto, User>().ReverseMap();

            CreateMap<Resource, ResourceDto>()
                .ForMember(
                    dest => dest.ResourceTypeName,
                    opt => opt.MapFrom(src => src.ResourceType.TypeName)
                )
                .ForMember(
                    dest => dest.ResourceTypeId,
                    opt => opt.MapFrom(src => src.ResourceTypeId)
                )
                .ReverseMap();

            CreateMap<ResourceForCreationDto, Resource>();
            CreateMap<ResourceForUpdateDto, Resource>().ReverseMap();

            CreateMap<Booking, BookingDto>().ReverseMap();
            CreateMap<BookingForCreationDto, Booking>();
            CreateMap<BookingForUpdateDto, Booking>().ReverseMap();

            CreateMap<ResourceType, ResourceTypeDto>().ReverseMap();
            CreateMap<ResourceTypeForCreationDto, ResourceType>();
            CreateMap<ResourceTypeForUpdateDto, ResourceType>().ReverseMap();
        }
    }
}
