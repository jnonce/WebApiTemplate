﻿using System;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using jnonce.WebApi.VersionedRouting;
using Owin;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    public partial class Startup
    {
        public event Action<HttpConfiguration> ConfiguringWebApi;

        public void ConfigureWebApi(
            IAppBuilder app,
            IContainer container)
        {
            var config = new HttpConfiguration();

            container.UpdateRegistrations(
                builder => ConfigureContainerForWebApi(config, builder)
                );

            // Allow config to modify WebApi
            if (ConfiguringWebApi != null)
            {
                ConfiguringWebApi(config);
            }

            // Enforce specific Json formatting
            config.Formatters.JsonFormatter.SerializerSettings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();

            // Routing: Use attribute based direct routes with Api constraints
            config.MapHttpAttributeRoutes(new ConstrainingDirectRouteProvider());

            // Validation
            config.UseAutofacFluentValidation(container);

            // Swagger (documentation)
            ConfigureDocs(app, container, config);

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
            builder.RegisterFluentValidators(asm);

            // Register the Api version provider
            builder.RegisterInstance<IApiVersionProvider>(
                new HttpHeaderApiVersionProvider("api-version"));
            builder.RegisterInstance<IApiVersionProvider>(
                new AcceptHeaderApiVersionProvider("vnd-api"));
        }
    }
}
