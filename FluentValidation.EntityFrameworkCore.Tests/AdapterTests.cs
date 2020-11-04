using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace FluentValidation.EntityFrameworkCore.Tests.Models
{
    [SuppressMessage("Design", "CA1063:Implement IDisposable Correctly", Justification = "By design")]
    public class AdapterTests : IDisposable
    {
        private readonly ServiceProvider services;
        private readonly EntityContext entityContext;

        public AdapterTests()
        {
            // arrange
            var serviceCollection = CreateServiceCollection();

            // act            
            serviceCollection
                .AddValidatorsFromAssemblyContaining<EntityValidator>();

            this.services = serviceCollection.BuildServiceProvider();
            this.entityContext = services.GetRequiredService<EntityContext>();
        }

        [Fact]
        public void Should_annotate_unrequired_property_nullable()
        {
            var entityType = entityContext.Model.FindEntityType(typeof(Entity));
            var nullProperty = entityType.FindProperty(nameof(Entity.Null));

            var expectedNullabilityValue = true;
            var actualNullabilityValue = nullProperty.IsNullable;

            Assert.Equal(expectedNullabilityValue, actualNullabilityValue);
        }

        [Fact]
        public void Should_annotate_required_property_not_nullable()
        {
            var entityType = entityContext.Model.FindEntityType(typeof(Entity));
            var notNullProperty = entityType.FindProperty(nameof(Entity.NotNull));

            var expectedNullabilityValue = false;
            var actualNullabilityValue = notNullProperty.IsNullable;

            Assert.Equal(expectedNullabilityValue, actualNullabilityValue);
        }

        [Fact]
        public void Should_annotate_max_length_property()
        {
            var entityType = entityContext.Model.FindEntityType(typeof(Entity));
            var maxLengthProperty = entityType.FindProperty(nameof(Entity.MaxLength10));

            var expectedMaxLength = 10;
            var actualMaxLength = maxLengthProperty.GetMaxLength();

            Assert.Equal(expectedMaxLength, actualMaxLength);
        }

        [Fact]
        public void Should_not_require_length_property()
        {
            var entityType = entityContext.Model.FindEntityType(typeof(Entity));
            var maxLengthProperty = entityType.FindProperty(nameof(Entity.MaxLength10));

            var expectedNullabilityValue = true;
            var actualNullabilityValue = maxLengthProperty.IsNullable;

            Assert.Equal(expectedNullabilityValue, actualNullabilityValue);
        }

        [Fact]
        public void Should_annotate_fixed_length_property()
        {
            var entityType = entityContext.Model.FindEntityType(typeof(Entity));
            var fixedLengthProperty = entityType.FindProperty(nameof(Entity.FixedLength10));

            var expectedFixedLength = 10;
            var expectedIsFixedLengthValue = true;

            var actualFixedLength = fixedLengthProperty.GetMaxLength();
            var actualIsFixedLengthValue = fixedLengthProperty.IsFixedLength();

            Assert.Equal(expectedFixedLength, actualFixedLength);
            Assert.Equal(expectedIsFixedLengthValue, actualIsFixedLengthValue);
        }

#if NET5_0
        [Fact]
        public void Should_annotate_precision_property_precision_value()
        {
            var entityType = entityContext.Model.FindEntityType(typeof(Entity));
            var precisonProperty = entityType.FindProperty(nameof(Entity.Precision5_Scale4));

            var expectedPrecisionValue = 5;
            var actualPrecisionValue = precisonProperty.GetPrecision();

            Assert.Equal(expectedPrecisionValue, actualPrecisionValue);
        }

        [Fact]
        public void Should_annotate_precision_property_scale_value()
        {
            var entityType = entityContext.Model.FindEntityType(typeof(Entity));
            var precisonProperty = entityType.FindProperty(nameof(Entity.Precision5_Scale4));

            var expectedScaleValue = 4;
            var actualScaleValue = precisonProperty.GetScale();

            Assert.Equal(expectedScaleValue, actualScaleValue);
        }
#endif

        private static IServiceCollection CreateServiceCollection() =>
            new ServiceCollection()
            .AddDbContext<EntityContext>(options =>
                options.UseInMemoryDatabase(Assembly.GetExecutingAssembly().FullName));

        public void Dispose() =>
            services.Dispose();
    }
}