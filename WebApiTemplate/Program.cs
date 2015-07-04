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
                webAppStart: cfgMethod => WebApp.Start(options, cfgMethod),
                delegatingServer: cfg.Start,
                serverWait: _ => GetConsoleCancel());
            serverExecution.Wait();
        }

        internal static async Task ServerAsync<TServer>(
            Func<Action<IAppBuilder>, TServer> webAppStart,
            Func<IAppBuilder, Func<IAppBuilder, Task>, Task> delegatingServer,
            Func<TServer, Task> serverWait)
            where TServer : IDisposable
        {
            // Task completes when server shuts down
            var serverFinished = new TaskCompletionSource<bool>();

            // Task completes when Configuration says we're ready
            var serverReadyToStart = new TaskCompletionSource<IAppBuilder>();

            // Task completes when startup/shutdown is finished
            Task configLifecycle = null;

            // Method which, when called sets a flag indicating the server is ready to process requests
            // Returns a task which indicates when the server is finished processing requests
            Func<IAppBuilder, Task> markServerReadyToStart =
                passedDownApp =>
                {
                    serverReadyToStart.SetResult(passedDownApp);
                    return serverFinished.Task;
                };

            // Define a method to configure the web application
            Action<IAppBuilder> configureWebApp =
                app =>
                {
                    // Call the startup method.
                    configLifecycle = delegatingServer(app, markServerReadyToStart);

                    // Wait for serverReadyToStart.
                    // Handle the case where serverReadyToStart is never achieved but configLifecycle completes
                    int index = Task.WaitAny(serverReadyToStart.Task, configLifecycle);
                    if (index != 0)
                    {
                        configLifecycle.Wait();
                        throw new OperationCanceledException();
                    }
                };

            // startServer calls WebApp.Start...
            using (TServer server = webAppStart(configureWebApp))
            {
                await serverWait(server);
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
