using System;
using System.Threading.Tasks;
using Autofac;

namespace webapitmpl.Utility
{
    internal static class IStartupAutofacExtensions
    {
        public static IComponentContext RunStartup<T>(this IComponentContext container)
            where T : IStartup
        {
            var item = container.Resolve<T>();
            item.Configuration();
            return container;
        }

        public static IComponentContext RunStartup<T>(this IComponentContext container, Action<T> preStartup)
            where T : IStartup
        {
            var item = container.Resolve<T>();
            preStartup(item);
            item.Configuration();
            return container;
        }
    }
}
