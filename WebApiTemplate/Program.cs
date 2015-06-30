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
            MainAsync().Wait();
        }

        private static Task MainAsync()
        {
            var cfg = ServiceConfiguration.GetCurrent();

            StartOptions options = new StartOptions();
            cfg.Configure(options);

            return ServerAsync(options, cfg.Configure, GetConsoleCancel);
        }

        private static async Task ServerAsync(
            StartOptions options,
            Func<IAppBuilder, Func<IAppBuilder, Task>, Task> startup,
            Func<Task> serverWait)
        {
            var serverFinished = new TaskCompletionSource<bool>();
            var serverReadyToStart = new TaskCompletionSource<bool>();
            Task configLifecycle = null;

            using (WebApp.Start(
                options,
                app =>
                {
                    configLifecycle = startup(
                        app,
                        app2 =>
                        {
                            serverReadyToStart.SetResult(true);
                            return serverFinished.Task;
                        });
                    serverReadyToStart.Task.Wait();
                }))
            {
                await serverWait();
            }

            if (configLifecycle != null)
            {
                serverFinished.SetResult(true);
                await configLifecycle;
            }
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
