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
            services.AddFluentValidation(Enumerable.Empty<Assembly>(), serviceLifetime);

        public static IServiceCollection AddFluentValidation(this IServiceCollection services, IEnumerable<Assembly> discoveryAssemblies, ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            if (discoveryAssemblies?.Any() != true)
                discoveryAssemblies = new[] { Assembly.GetCallingAssembly() };

            return services
                .AddValidatorsFromAssemblies(discoveryAssemblies, serviceLifetime)
                .AddTransient<IValidatorFactory, DefaultValidatorFactory>();
        }
    }
}