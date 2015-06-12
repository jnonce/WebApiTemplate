using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using FluentValidation;
using FluentValidation.Attributes;
using FluentValidation.WebApi;
using Owin;
using webapitmpl.Configuration;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public void ConfigureWebApi(
            IAppBuilder app,
            IServiceConfiguration svcConfig,
            IContainer container)
        {
            var config = new HttpConfiguration();

            container.UpdateRegistrations(
                builder => ConfigureContainerForWebApi(config, builder)
                );

            // Allow config to modify WebApi
            svcConfig.Configure(config);

            // Routing
            config.MapHttpAttributeRoutes();

            // Validation            
            FluentValidationModelValidatorProvider.Configure(
                config,
                provider =>
                {
                    provider.ValidatorFactory = new AutofacFluentValidatorFactory(container);
                });

            // Pull the OWIN dependency scope into WebApi's request state
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            app.UseAutofacWebApi(config);

            // Place WebApi onto the Owin pipeline
            app.UseWebApi(config);
        }

        private static void ConfigureContainerForWebApi(HttpConfiguration config, ContainerBuilder builder)
        {
            // Allow Autofac to setup filters
            builder.RegisterWebApiFilterProvider(config);

            // Autofac will make HttpRequestMessage available
            builder.RegisterHttpRequestMessage(config);

            // Autofac will create controllers
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            builder.RegisterApiControllers(asm);

            // Autofac will create validators
            ConfigureValidation(asm, builder);
        }

        private static void ConfigureValidation(Assembly asm, ContainerBuilder builder)
        {
            // Find model types which have ValidatorAttribute
            // Group the items by the ValidatorAttribute's type
            IEnumerable<IGrouping<Type, Type>> validatorTypes = asm.GetTypes()
                .SelectMany(
                    type => type.GetCustomAttributes(typeof(ValidatorAttribute), inherit: false),
                    (type, item) => new
                    {
                        ModelType = type,
                        ValidatorType = ((ValidatorAttribute)item).ValidatorType
                    })
                .GroupBy(x => x.ValidatorType, x => x.ModelType);

            // Register these validators
            Type validator = typeof(IValidator<>);
            foreach (IGrouping<Type, Type> validatorToModels in validatorTypes)
            {
                // Define the validator as a component
                var registration = builder.RegisterType(validatorToModels.Key);

                // Register each model as an service provided
                foreach (Type modelType in validatorToModels)
                {
                    registration = registration.As(validator.MakeGenericType(modelType));
                }
            }
        }
    }
}
