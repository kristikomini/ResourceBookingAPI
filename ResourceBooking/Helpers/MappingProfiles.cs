using AutoMapper;
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

            // The public booking DTOs use the Italian field names DataInizio/DataFine,
            // while the entity uses StartDate/EndDate, so the mapping must be explicit.
            CreateMap<Booking, BookingDto>()
                .ForMember(dest => dest.DataInizio, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.DataFine, opt => opt.MapFrom(src => src.EndDate));
            CreateMap<BookingDto, Booking>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.DataInizio))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.DataFine));
            CreateMap<BookingForCreationDto, Booking>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.DataInizio))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.DataFine));
            CreateMap<BookingForUpdateDto, Booking>().ReverseMap();

            CreateMap<ResourceType, ResourceTypeDto>().ReverseMap();
            CreateMap<ResourceTypeForCreationDto, ResourceType>();
            CreateMap<ResourceTypeForUpdateDto, ResourceType>().ReverseMap();

            // Open-generic map so a PaginatedResult<TSource> can project to
            // PaginatedResult<TDestination> (e.g. PaginatedResult<Resource> -> PaginatedResult<ResourceDto>).
            CreateMap(typeof(PaginatedResult<>), typeof(PaginatedResult<>));
        }
    }
}
