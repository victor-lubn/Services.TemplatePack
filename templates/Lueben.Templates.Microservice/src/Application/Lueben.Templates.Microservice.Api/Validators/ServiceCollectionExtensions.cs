using System.Reflection;
using Lueben.Microservice.Api.ValidationFunction;
using Microsoft.Extensions.DependencyInjection;

namespace Lueben.Templates.Microservice.Api.Validators
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFluentValidators(this IServiceCollection services)
        {
            var assemblyWithValidators = Assembly.GetAssembly(typeof(ApplicationModelValidator));
            return services.AddValidators(new[] { assemblyWithValidators });
        }
    }
}