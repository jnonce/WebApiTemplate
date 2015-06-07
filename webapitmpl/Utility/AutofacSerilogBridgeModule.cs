using System;
using System.Linq;
using Autofac;
using Autofac.Core;
using Serilog;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Bridge Serilog <see cref="ILogger"/> through Autofac.
    /// </summary>
    public class AutofacSerilogBridgeModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Make the global logger available for use (if not already registered)
            builder.Register(c => Log.Logger)
                .ExternallyOwned()
                .PreserveExistingDefaults();

            base.Load(builder);
        }

        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            registration.Preparing += registration_Preparing;
        }

        // Ensure ILogger injection has type information associated
        private void registration_Preparing(object sender, PreparingEventArgs e)
        {
            Type limitType = e.Component.Activator.LimitType;
            var parameter = new ResolvedParameter(
                (p, i) => p.ParameterType == typeof(ILogger),
                (p, i) => e.Context.Resolve<ILogger>().ForContext(limitType));

            e.Parameters = e.Parameters.Concat(new[] { parameter });
        }
    }
}
