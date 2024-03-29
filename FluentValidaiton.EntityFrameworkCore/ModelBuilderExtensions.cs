﻿using System;
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

        public static void ApplyConfigurationFromFluentValidations(this ModelBuilder modelBuilder, IValidatorFactory validatorFactory)
        {
            modelBuilder.ApplyConfigurationFromFluentValidations(entityType => validatorFactory.GetValidator(entityType));
        }

        public static void ApplyConfigurationFromFluentValidations(this ModelBuilder modelBuilder, IServiceProvider serviceProvider)
        {
            IValidator validatorFactory(Type entityType)
            {
                var genericValidatorType = ValidatorType.MakeGenericType(entityType);
                return (IValidator)serviceProvider.GetRequiredService(genericValidatorType);
            }

            modelBuilder.ApplyConfigurationFromFluentValidations(validatorFactory);
        }

        private static void ApplyConfigurationFromFluentValidations(this ModelBuilder modelBuilder, Func<Type, IValidator?> validatorFactory)
        {
            var entityTypes =
                modelBuilder
                .Model
                .GetEntityTypes()
                .Where(entityType => entityType.ClrType != null)
                .Select(entityType => entityType.ClrType);

            foreach (var entityType in entityTypes)
            {
                var validator = validatorFactory(entityType);

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
                            getPropertyBuilder(propertyName)
                                .HasMaxLength(exactLengthValidator.Max)
                                .IsFixedLength();
                            break;
                        case ILengthValidator lengthValidator when lengthValidator.Max > 0:
                            getPropertyBuilder(propertyName).HasMaxLength(lengthValidator.Max);
                            break;
                        case ScalePrecisionValidator scalePrecisionValidator:
                            var propertyBuilder = getPropertyBuilder(propertyName);

                            if (propertyBuilder.Metadata.ClrType == typeof(decimal))
                            {
#if NETSTANDARD2_1 || NET6_0 //for lower versions - set with annotations etc.
                                propertyBuilder
                                    .HasPrecision(
                                       precision: scalePrecisionValidator.Precision,
                                       scale: scalePrecisionValidator.Scale);
#else
                                //if sql-server/pgs
                                //propertyBuilder
                                //    .HasColumnType($"decimal({scalePrecisionValidator.Precision},{scalePrecisionValidator.Scale})");
#endif
                            }
                            break;
                    }
                }
            }
        }
    }
}