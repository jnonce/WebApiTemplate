using System;
using System.Threading;
using Owin;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Extensions for <see cref="IAppBuilder"/> adding dispose event handling
    /// </summary>
    public static class AppBuilderDisposeExtensions
    {
        /// <summary>
        /// Registers a disposable to be tracked by the application disposing.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="disposable">The disposable.</param>
        public static void RegisterAppDisposing(this IAppBuilder app, IDisposable disposable)
        {
            app.RegisterAppDisposing(disposable.Dispose);
        }

        /// <summary>
        /// Registers an action to call at the application disposing.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="action">The action.</param>
        public static void RegisterAppDisposing(this IAppBuilder app, Action action)
        {
            CancellationToken token;
            if (TryGetCancellationToken(app, out token))
            {
                token.Register(action);
            }
        }

        /// <summary>
        /// Gets the application's disposing <see cref="CancellationToken"/>.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <returns>The cancellation token</returns>
        public static CancellationToken GetAppDisposingToken(this IAppBuilder app)
        {
            CancellationToken token;
            TryGetCancellationToken(app, out token);
            return token;
        }

        private static bool TryGetCancellationToken(IAppBuilder app, out CancellationToken token)
        {
            object o;
            if (app.Properties.TryGetValue("host.OnAppDisposing", out o)
                && o is CancellationToken)
            {
                token = (CancellationToken)o;
                return true;
            }
            else
            {
                token = CancellationToken.None;
                return false;
            }
        }
    }
}
