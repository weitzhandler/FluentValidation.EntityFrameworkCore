using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace FluentValidaiton.EntityFrameworkCore
{
    public static class ModelBuilderExtensions
    {
        private static readonly Type ValidatorType = typeof(IValidator<>);

        public static void ApplyConfigurationFromFluentValidations(this ModelBuilder modelBuilder, IServiceProvider serviceProvider)
        {
            var entityTypes =
                modelBuilder
                .Model
                .GetEntityTypes()
                .Where(entityType => entityType.HasClrType())
                .Select(entityType => entityType.ClrType);

            foreach (var entityType in entityTypes)
            {
                var genericValidatorType = ValidatorType.MakeGenericType(entityType);
                var validator = (IValidator)serviceProvider.GetRequiredService(genericValidatorType);

                if (validator == null)
                    continue;

                var entityTypeBuilder = modelBuilder.Entity(entityType);
                var descriptor = validator.CreateDescriptor();

                ApplyConfigurations(entityTypeBuilder, descriptor);
            }
        }

        private static void ApplyConfigurations(EntityTypeBuilder entityTypeBuilder, IValidatorDescriptor validatorDescriptor)
        {
            var allPropetyValidators =
                validatorDescriptor
                .GetMembersWithValidators();

            Dictionary<string, PropertyBuilder> propertyBuilders = new Dictionary<string, PropertyBuilder>();
            PropertyBuilder getPropertyBuilder(string propertyName)
            {
                if (!propertyBuilders.TryGetValue(propertyName, out var propertyBuilder))
                {
                    propertyBuilder = entityTypeBuilder.Property(propertyName);
                    propertyBuilders[propertyName] = propertyBuilder;
                }

                return propertyBuilder;
            }

            foreach (var propertyValidators in allPropetyValidators)
            {
                var propertyName = propertyValidators.Key;

                foreach (var propertyValidator in propertyValidators)
                {
                    switch (propertyValidator)
                    {
                        case INotNullValidator notNullValidator:
                        case INotEmptyValidator notEmptyValidator:
                            getPropertyBuilder(propertyName)
                                .IsRequired();
                            break;
                        case ExactLengthValidator exactLengthValidator:
                            //set to char/nchar(length) according to exsisting type                            
                            //or via annotations
                            break;
                        case ILengthValidator lengthValidator when lengthValidator.Max > 0:
                            getPropertyBuilder(propertyName).HasMaxLength(lengthValidator.Max);
                            break;
#if NETSTANDARD2_1 //for lower versions - set with annotations etc.
                        case ScalePrecisionValidator scalePrecisionValidator:
                            var propertyBuilder = getPropertyBuilder(propertyName);

                            if (propertyBuilder.Metadata.ClrType == typeof(decimal))
                            {
                                propertyBuilder
                                    .HasPrecision(
                                       precision: scalePrecisionValidator.Precision,
                                       scale: scalePrecisionValidator.Scale);
                            }
                            break;
#endif
                    }
                }
            }
        }
    }
}