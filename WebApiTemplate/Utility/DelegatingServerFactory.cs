using System;
using System.Linq;
using System.Threading.Tasks;
using Owin;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Sequence a series of tasks in a chain
    /// </summary>
    public static class DelegatingServerFactory
    {
        /// <summary>
        /// Creates the server.
        /// </summary>
        /// <param name="innerServer">The inner server.</param>
        /// <param name="delegatingServers">The delegating servers which wrap the innerServer.</param>
        /// <returns>Final server, with all wrapping</returns>
        public static Func<IAppBuilder, Task> CreateServer(Func<IAppBuilder, Task> innerServer, params IDelegatingServer[] delegatingServers)
        {
            return delegatingServers.Reverse().Aggregate(
                innerServer,
                (runServerHere, starter) =>
                {
                    return (app) => starter.Start(app, runServerHere);
                }
                );
        }

        /// <summary>
        /// Creates a delegating server from a delegate.
        /// </summary>
        /// <param name="server">The server method.</param>
        /// <returns>Delegating server</returns>
        public static IDelegatingServer Create(Func<IAppBuilder, Func<IAppBuilder, Task>, Task> server)
        {
            return new DelegatingServer(server);
        }

        private sealed class DelegatingServer : IDelegatingServer
        {
            private Func<IAppBuilder, Func<IAppBuilder, Task>, Task> server;

            public DelegatingServer(Func<IAppBuilder, Func<IAppBuilder, Task>, Task> server)
            {
                this.server = server;
            }

            public Task Start(IAppBuilder app, Func<IAppBuilder, Task> innerServer)
            {
                return this.server(app, innerServer);
            }
        }
    }
}
