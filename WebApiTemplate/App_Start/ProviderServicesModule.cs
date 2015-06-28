using Autofac;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Basic services used by controllers.
    /// </summary>
    /// <remarks>
    /// This is an example module.  You can extend this or replace it.
    /// </remarks>
    internal class ProviderServicesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register service types
            builder.RegisterType<webapitmpl.Providers.DemoProvider>()
                .InstancePerRequest();

            builder.RegisterType<Microsoft.Owin.Infrastructure.SystemClock>()
                .As<Microsoft.Owin.Infrastructure.ISystemClock>();
        }
    }
}
