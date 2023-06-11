using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
    public class CommandsService : AutoMapper.Profile
    {
        public class CommandsProfile : AutoMapper.Profile
        {
            public CommandsProfile()
            {
                // Source -> Target
                CreateMap<Platform, PlatformReadDto>();
                CreateMap<CommandCreateDto, Command>();
                CreateMap<Command, CommandReadDto>();
                // This maps Id of dto to external id of source
                CreateMap<PlatformPublishedDto, Platform>()
                    .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.Id));
                CreateMap<GrpcPlatformModel, Platform>()
                .ForMember(dest => dest.ExternalID, opt => opt.MapFrom(src => src.PlatformId))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Commands, opt => opt.Ignore());
            }
        }
    }
}