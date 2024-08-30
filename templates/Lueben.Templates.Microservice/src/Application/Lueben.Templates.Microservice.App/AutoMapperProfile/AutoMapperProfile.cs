using AutoMapper;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.CreateApplication;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication;
using Lueben.Templates.Microservice.Domain.Entities;

namespace Lueben.Templates.Microservice.App.AutoMapperProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // #if (Cosmos)
            CreateMap<CreateApplicationCommandCosmos, Application>().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdateApplicationCommandCosmos, Application>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<PartiallyUpdateApplicationCommandCosmos, Application>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            // #else
            CreateMap<CreateApplicationCommandSql, Application>().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdateApplicationCommandSql, Application>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            CreateMap<PartiallyUpdateApplicationCommandSql, Application>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
            // #endif
        }
    }
}
