using Autofac;
using Owin;

namespace webapitmpl.App_Start
{
    /// <summary>
    /// Register Owin related services
    /// </summary>
    internal class OwinServicesModule : Module
    {
        public IAppBuilder AppBuilder { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(AppBuilder).ExternallyOwned();
        }
    }
}
