using AutoMapper;
using LicensePlateClient.Services.Base;

namespace LicensePlateClient.Configuration;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<LicensePlateReadOnlyDto, LicensePlateUpdateDto>().ReverseMap();
    }
}