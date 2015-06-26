using System;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using jnonce.WebApi.VersionedRouting;
using webapitmpl.Utility;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Define the HttpConfiguration and core services needed by WebAPI
    /// </summary>
    class WebApiServicesModule : Module
    {
        public Action<HttpConfiguration> ConfiguringWebApi;

        protected override void Load(ContainerBuilder builder)
        {
            var config = new HttpConfiguration();

            builder.RegisterInstance(config).ExternallyOwned();

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

            // Register a starter to insert webapi into the pipeline
            builder.RegisterType<WebApiStarter>().SingleInstance().As<IStartable>();
        }
    }
}
