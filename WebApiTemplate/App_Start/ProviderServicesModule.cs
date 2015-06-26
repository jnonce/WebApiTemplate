using Autofac;

namespace webapitmpl.App_Start
{
    class ProviderServicesModule : Module
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
