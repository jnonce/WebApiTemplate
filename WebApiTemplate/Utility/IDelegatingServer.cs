using System;
using System.Threading.Tasks;
using Owin;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Server startup class
    /// </summary>
    public interface IDelegatingServer
    {
        /// <summary>
        /// Runs the server
        /// </summary>
        /// <param name="next">The innerServer to execute</param>
        /// <returns>Task indicating when the server is complete.</returns>
        Task Start(IAppBuilder app, Func<IAppBuilder, Task> innerServer);
    }
}
