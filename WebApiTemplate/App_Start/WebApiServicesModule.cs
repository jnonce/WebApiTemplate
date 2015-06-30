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

            // Register a starter to insert webapi into the pipeline
            builder.RegisterType<WebApiStartup>()
                .AsSelf()
                .Keyed<IStartup>(WebApiStartup.Id);
        }
    }
}
