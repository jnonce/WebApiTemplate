using System;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using Owin;
using webapitmpl.Configuration;

namespace webapitmpl
{
    class Program
    {
        private static void Main(string[] args)
        {
            var cfg = ServiceConfiguration.GetCurrent();

            StartOptions options = new StartOptions();
            cfg.Configure(options);

            Task serverExecution = ServerAsync(
                options,
                cfg.Configure,
                GetConsoleCancel);
            serverExecution.Wait();
        }

        internal static Task ServerAsync(
            StartOptions options,
            Func<IAppBuilder, Func<IAppBuilder, Task>, Task> startup,
            Func<Task> serverWait)
        {
            return ServerAsync(
                cfgMethod => WebApp.Start(options, cfgMethod),
                startup,
                _ => serverWait());
        }

        internal static async Task ServerAsync<TServer>(
            Func<Action<IAppBuilder>, TServer> startServer,
            Func<IAppBuilder, Func<IAppBuilder, Task>, Task> startup,
            Func<TServer, Task> serverWait)
            where TServer : IDisposable
        {
            // Task completes when server shuts down
            var serverFinished = new TaskCompletionSource<bool>();

            // Task completes when Configuration says we're ready
            var serverReadyToStart = new TaskCompletionSource<bool>();

            // Task completes when startup/shutdown is finished
            Task configLifecycle = null;

            TServer server = startServer(
                app =>
                {
                    configLifecycle = startup(
                        app,
                        _ =>
                        {
                            serverReadyToStart.SetResult(true);
                            return serverFinished.Task;
                        });
                    WaitAny(serverReadyToStart.Task, configLifecycle);
                });

            using (server)
            {
                await serverWait(server);
            }

            if (configLifecycle != null)
            {
                serverFinished.SetResult(true);
                await configLifecycle;
            }
        }

        private static void WaitAny(params Task[] tasks)
        {
            int index = Task.WaitAny(tasks);
            tasks[index].Wait();
        }

        private static Task GetConsoleCancel()
        {
            var complete = new TaskCompletionSource<object>();
            Console.CancelKeyPress += new ConsoleCancelEventHandler(
                (sender, e) =>
                {
                    e.Cancel = true;
                    complete.SetResult(null);
                });
            return complete.Task;
        }
    }
}
