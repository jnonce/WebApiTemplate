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
    /// <remarks>
    /// This module ensures that the ILogger received by a component is set to the type's context
    /// </remarks>
    public class AutofacSerilogBridgeModule : Module
    {
        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">
        /// The builder through which components can be registered.
        /// </param>
        /// <remarks>
        /// Note that the ContainerBuilder parameter is unique to this module.
        /// </remarks>
        protected override void Load(ContainerBuilder builder)
        {
            // Make the global logger available for use (if not already registered)
            builder.Register(c => Log.Logger)
                .ExternallyOwned()
                .PreserveExistingDefaults();

            base.Load(builder);
        }

        /// <summary>
        /// Override to attach module-specific functionality to a
        /// component registration.
        /// </summary>
        /// <param name="componentRegistry">The component registry.</param>
        /// <param name="registration">The registration to attach functionality to.</param>
        /// <remarks>
        /// This method will be called for all existing <i>and future</i> component
        /// registrations - ordering is not important.
        /// </remarks>
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
