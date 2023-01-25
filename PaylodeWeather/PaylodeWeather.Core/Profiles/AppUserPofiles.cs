using AutoMapper;
using PaylodeWeather.Core.Dtos;
using PaylodeWeather.Domain.Model;

namespace WeatherApi.Core.Utilities
{
    public class AppUserPofiles : Profile
    {
        public AppUserPofiles()
        {
            CreateMap<RegistrationDTO, AppUser>()
                 .ForMember(dest => dest.Email, act => act.MapFrom(src => src.Email.ToLower()))
                 .ForMember(dest => dest.UserName, act => act.MapFrom(src => src.Email.ToLower()));
        }
    }
}
