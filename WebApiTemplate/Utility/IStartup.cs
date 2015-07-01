using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Owin;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Startup class
    /// </summary>
    public interface IStartup
    {
        /// <summary>
        /// Configures the Owin pipeline
        /// </summary>
        Task Configuration(Func<Task> next);
    }
}
