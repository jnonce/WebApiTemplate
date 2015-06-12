using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using FluentValidation.WebApi;
using Owin;
using webapitmpl.Configuration;
using webapitmpl.Utility;
using FluentValidation;
using System.Text.RegularExpressions;

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
            FluentValidationModelValidatorProvider.Configure(config,
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

            //
            builder.RegisterAssemblyTypes(asm)
                .Where(t => typeof(FluentValidation.IValidator).IsAssignableFrom(t))
                .As(type => GetTypes(type))
                ;
        }

        private static IEnumerable<Type> GetTypes(Type validatorType)
        {
            var interfaces = new List<Type>();

            // Match the convention on the validator name
            Match match = Regex.Match(validatorType.Name, @"(\w+)Validator", RegexOptions.CultureInvariant);
            if (match.Success)
            {
                // Locate the targetType
                string targetType = String.Format("{0}.{1}", validatorType.Namespace, match.Groups[1].Value);
                Type type = validatorType.Assembly.GetType(targetType, throwOnError: false);
                if (type != null)
                {
                    Type desiredType = typeof(IValidator<>).MakeGenericType(type);
                    if (desiredType.IsAssignableFrom(validatorType))
                    {
                        interfaces.Add(desiredType);
                    }
                }
            }

            return interfaces;
        }
    }
}
