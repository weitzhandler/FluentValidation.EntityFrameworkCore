using System;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidaiton.EntityFrameworkCore
{
    public class DefaultValidatorFactory : IValidatorFactory
    {
        readonly IServiceProvider serviceProvider;
        static readonly Type genericValidatorType = typeof(IValidator<>);

        public DefaultValidatorFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IValidator<T>? GetValidator<T>() => serviceProvider.GetService<IValidator<T>>();

        public IValidator? GetValidator(Type validatedEntityType)
        {
            var validatorType = genericValidatorType.MakeGenericType(validatedEntityType);

            return (IValidator?)serviceProvider.GetService(validatorType);
        }
    }
}