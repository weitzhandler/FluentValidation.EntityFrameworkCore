using System.Linq;
using FluentValidaiton.EntityFrameworkCore;
using FluentValidation.EntityFrameworkCore.Tests.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FluentValidation.EntityFrameworkCore.Tests
{
    public class ValidationFactoryExtensionsTests
    {
        //[Fact]
        public void Should_register_from_calling_assembly()
        {
            // arrange
            var services = new ServiceCollection();

            // act
            services.AddFluentValidation();

            // assert
            services.Any(descriptor =>
                descriptor.ServiceType == typeof(IValidator<Entity>)
                && descriptor.ImplementationType == typeof(EntityValidator)
                && descriptor.Lifetime == ServiceLifetime.Transient);

            services.Any(descriptor =>
                descriptor.ServiceType == typeof(IValidatorFactory)
                && descriptor.ImplementationType == typeof(DefaultValidatorFactory)
                && descriptor.Lifetime == ServiceLifetime.Transient);
        }
    }
}