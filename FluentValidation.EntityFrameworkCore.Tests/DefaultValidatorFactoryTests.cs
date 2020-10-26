using System;
using FluentValidaiton.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FluentValidation.EntityFrameworkCore.Tests
{
    public class DefaultValidatorFactoryTests
    {
        readonly Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>(MockBehavior.Strict);
        readonly IValidatorFactory validatorFactory;

        public DefaultValidatorFactoryTests()
        {
            validatorFactory = new DefaultValidatorFactory(serviceProviderMock.Object);
        }

        [Fact]
        public void Should_resolve_validator()
        {
            // arrange
            var validator = Mock.Of<IValidator<int>>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidator<int>)))
                .Returns(validator);

            // act
            var actualValidator = validatorFactory.GetValidator(typeof(int));

            // assert
            Assert.Same(validator, actualValidator);
        }

        [Fact]
        public void Should_resolve_validator_generic()
        {
            // arrange
            var validator = Mock.Of<IValidator<int>>();
            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidator<int>)))
                .Returns(validator);

            // act
            var actualValidator = validatorFactory.GetValidator<int>();

            // assert
            Assert.Same(validator, actualValidator);
        }

        [Fact]
        public void Should_not_throw_on_non_registered()
        {
            // arrange

            serviceProviderMock
                .Setup(serviceProvider => serviceProvider.GetService(typeof(IValidator<int>)))
                .Returns(null);

            // act
            var actualValidator = validatorFactory.GetValidator<int>();

            // assert
            Assert.Null(actualValidator);
        }
    }
}