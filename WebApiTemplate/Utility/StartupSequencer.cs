using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webapitmpl.Utility
{
    /// <summary>
    /// Sequence a series of tasks in a chain
    /// </summary>
    public class StartupSequencer : System.Collections.IEnumerable
    {
        private ICollection<Func<Func<Task>, Task>> chain = new List<Func<Func<Task>, Task>>();

        /// <summary>
        /// Executes the startup sequence.
        /// </summary>
        /// <param name="last">The last operation to execute at the end of the chain</param>
        /// <returns></returns>
        public Task Execute(Func<Task> last)
        {
            return chain.Reverse().Aggregate(
                last,
                (runServerHere, starter) =>
                {
                    return () => starter(runServerHere);
                }
                )();
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(IStartup item)
        {
            chain.Add(item.Configuration);
        }

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(Func<Func<Task>, Task> item)
        {
            chain.Add(item);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return chain.GetEnumerator();
        }
    }
}
