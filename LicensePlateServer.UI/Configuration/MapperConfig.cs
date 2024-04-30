using AutoMapper;
using LicensePlateServer.UI.Services.Base;

namespace LicensePlateServer.UI.Configuration;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<LicensePlateReadOnlyDto, LicensePlateUpdateDto>().ReverseMap();
    }
}