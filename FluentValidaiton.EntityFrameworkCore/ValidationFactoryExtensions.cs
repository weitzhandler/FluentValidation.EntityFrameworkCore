using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidaiton.EntityFrameworkCore
{
    public static class ValidationFactoryExtensions
    {
        public static IServiceCollection AddFluentValidation(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Transient) =>
            services.AddFluentValidation(new[] { Assembly.GetCallingAssembly() }, serviceLifetime);

        public static IServiceCollection AddFluentValidation(this IServiceCollection services, IEnumerable<Assembly> discoveryAssemblies, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (discoveryAssemblies == null)
                throw new ArgumentNullException(nameof(discoveryAssemblies));

            if (!discoveryAssemblies.Any())
                throw new ArgumentException("No assemblies were specified."); ;

            return services
                .AddValidatorsFromAssemblies(discoveryAssemblies, serviceLifetime)
                .AddTransient<IValidatorFactory, DefaultValidatorFactory>();
        }
    }
}