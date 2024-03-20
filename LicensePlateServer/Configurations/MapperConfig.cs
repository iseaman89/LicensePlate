using AutoMapper;
using LicensePlateDataShared.Models;
using LicensePlateServer.Data;
using LicensePlateServer.Models.Cameras;
using LicensePlateServer.Models.LicensePlates;
using LicensePlateServer.Models.Users;

namespace LicensePlateServer.Configurations;

public class MapperConfig : Profile
{
    public MapperConfig()
    {
        CreateMap<CameraCreateDto, Camera>().ReverseMap();
        CreateMap<CameraReadOnlyDto, Camera>().ReverseMap();
        CreateMap<CameraUpdateDto, Camera>().ReverseMap();

        CreateMap<LicensePlateCreateDto, LicensePlate>().ReverseMap();
        CreateMap<LicensePlateReadOnlyDto, LicensePlate>().ReverseMap();
        CreateMap<LicensePlateUpdateDto, LicensePlate>().ReverseMap();
        
        CreateMap<ApiUser, UserDto>().ReverseMap();
    }
}