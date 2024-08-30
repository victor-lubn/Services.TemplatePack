using AutoMapper;
using Lueben.Templates.Microservice.Api.Contract.Models;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.CreateApplication;
using Lueben.Templates.Microservice.App.UseCases.Applications.Commands.UpdateApplication;

namespace Lueben.Templates.Microservice.Api.AutoMapperProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Domain.Entities.Application, Application>().ForMember(dest => dest.ApplicationId, opt => opt.MapFrom(src => src.Id));
            // #if (Cosmos)
            CreateMap<Application, CreateApplicationCommandCosmos>();
            CreateMap<Application, PartiallyUpdateApplicationCommandCosmos>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApplicationId));
            CreateMap<Application, UpdateApplicationCommandCosmos>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApplicationId));
            // #else
            CreateMap<Application, CreateApplicationCommandSql>();
            CreateMap<Application, PartiallyUpdateApplicationCommandSql>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApplicationId));
            CreateMap<Application, UpdateApplicationCommandSql>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ApplicationId));
            // #endif
        }
    }
}