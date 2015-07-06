using System;
using System.Threading;
using System.Threading.Tasks;
using Owin;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Startup class which delegates to a <see cref="IDelegatingServer"/>
    /// </summary>
    public class DelegatingServerStartup
    {
        /// <summary>
        /// Gets or sets the delegating server.
        /// </summary>
        /// <value>
        /// The delegating server.
        /// </value>
        public IDelegatingServer DelegatingServer { get; set; }

        /// <summary>
        /// Configurations the specified application.
        /// </summary>
        /// <param name="app">The application to configure.</param>
        public void Configuration(IAppBuilder app)
        {
            IDelegatingServer server = GetDelegatingServer(app);

            // Event triggered when the server should start taking requests.
            var readyForRequests = new TaskCompletionSource<IAppBuilder>();

            // Task which tracks the lifespan of the inner server.
            var serverComplete = new TaskCompletionSource<bool>();

            // Pass to the delegating server
            Task shutdownComplete = server.Start(app,
                passedApp =>
                {
                    if (passedApp != app)
                    {
                        throw new ArgumentException("app");
                    }

                    readyForRequests.TrySetResult(passedApp);
                    return serverComplete.Task;
                });

            // Wait for the delegating server to respond and allow incoming requests
            if (Task.WaitAny(readyForRequests.Task, shutdownComplete) == 1)
            {
                // If the delegating server completed without allowing incoming requests
                // Then AppStart needs to terminate.  Allow exception trapped by the task
                // to bubble out, otherwise throw an error of our own.
                shutdownComplete.Wait();
                throw new OperationCanceledException();
            }

            //
            // Register a tracker for app disposing.
            // When this happens, allow the delegating server to shutdown and wait for it to do so.
            CancellationToken token;
            if (TryGetCancellationToken(app, out token))
            {
                token.Register(
                    () =>
                    {
                        serverComplete.SetResult(true);
                        shutdownComplete.Wait();
                    });
            }
        }

        private IDelegatingServer GetDelegatingServer(IAppBuilder app)
        {
            if (this.DelegatingServer != null)
            {
                return this.DelegatingServer;
            }

            object value;
            if (app.Properties.TryGetValue(typeof(IDelegatingServer).FullName, out value))
            {
                string serverName = value as string;
                if (serverName != null)
                {
                    Type type = Type.GetType(serverName, throwOnError: true);
                    return (IDelegatingServer)Activator.CreateInstance(type);
                }

                return (IDelegatingServer)value;
            }

            throw new InvalidOperationException();
        }

        // Retrieve the cancellation token indicating when the IAppBuilder's host is terminating
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
