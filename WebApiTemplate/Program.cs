using System;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using webapitmpl.Configuration;

namespace webapitmpl
{
    class Program
    {
        private static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            var cfg = ServiceConfiguration.GetCurrent();

            StartOptions options = new StartOptions();
            cfg.Configure(options);

            var serverReadyToStart = new TaskCompletionSource<bool>();
            var serverFinished = new TaskCompletionSource<bool>();
            Task configLifecycle = null;

            using (WebApp.Start(
                options,
                app =>
                {
                    configLifecycle = cfg.Configure(app, app2 =>
                        {
                            serverReadyToStart.SetResult(true);
                            return serverFinished.Task;
                        });
                    serverReadyToStart.Task.Wait();
                }))
            {
                await GetConsoleCancel();
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
