using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.WebApi;
using Assembly = System.Reflection.Assembly;

namespace webapitmpl.Utility
{
    internal static class AutofacFluentValidationExtensions
    {
        /// <summary>
        /// Registers the fluent validators.
        /// </summary>
        /// <param name="builder">The builder into which to register.</param>
        /// <param name="assemblies">The assemblies from which to find model types.</param>
        /// <remarks>
        /// Scan for classes with <see cref="ValidatorAttribute"/> applied.  The ValidatorType
        /// specified is registered as a component and provides the appropriate <see cref="T:IValidator`1"/>
        /// service.
        /// </remarks>
        public static void RegisterFluentValidators(
            this ContainerBuilder builder, 
            params Assembly[] assemblies)
        {
            // Find model types which have ValidatorAttribute
            // Group the items by the ValidatorAttribute's type
            var validatorTypes =
                assemblies
                .SelectMany(asm => asm.GetTypes())
                .SelectMany(
                    type => type.GetCustomAttributes(typeof(ValidatorAttribute), inherit: false),
                    (type, item) => new
                    {
                        ModelType = type,
                        ValidatorType = ((ValidatorAttribute)item).ValidatorType
                    })
                .GroupBy(
                    x => x.ValidatorType, 
                    x => x.ModelType,
                    (validatorType, modelTypes) => new 
                    { 
                        ValidatorType = validatorType, 
                        ModelTypes = modelTypes.ToArray() 
                    });

            // Register these validators
            Type validator = typeof(IValidator<>);
            foreach (var validatorToModels in validatorTypes)
            {
                // Define the validator as a component
                var registration = builder.RegisterType(validatorToModels.ValidatorType);

                // Register each model as an service provided
                foreach (Type modelType in validatorToModels.ModelTypes)
                {
                    registration = registration.As(validator.MakeGenericType(modelType));
                }
            }
        }

        /// <summary>
        /// Uses the Autofac provided validators for fluent validation within WebApi.
        /// </summary>
        /// <param name="config">The WebAPI Http configuration.</param>
        /// <param name="container">The container from which validators will be retrieved.</param>
        public static void UseAutofacFluentValidation(this System.Web.Http.HttpConfiguration config, IContainer container)
        {
            FluentValidationModelValidatorProvider.Configure(
                config,
                provider =>
                {
                    provider.ValidatorFactory = new AutofacFluentValidatorFactory(container);
                });
        }
    }
}
