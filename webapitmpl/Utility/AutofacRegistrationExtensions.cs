using System;
using Autofac;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Utilities to improve container update readability
    /// </summary>
    internal static class AutofacRegistrationExtensions
    {
        /// <summary>
        /// Updates the registrations in a container.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="builder">The builder.</param>
        public static void UpdateRegistrations(
            this IContainer container,
            Action<ContainerBuilder> builder)
        {
            var b = new ContainerBuilder();
            builder(b);
            b.Update(container);
        }

        /// <summary>
        /// Updates the registrations in a lifetime scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="builder">The builder.</param>
        public static void UpdateRegistrations(
            this ILifetimeScope scope,
            Action<ContainerBuilder> builder)
        {
            var b = new ContainerBuilder();
            builder(b);
            b.Update(scope.ComponentRegistry);
        }
    }
}
