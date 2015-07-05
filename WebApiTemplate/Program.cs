using System;
using System.Threading;
using Microsoft.Owin.Hosting;
using webapitmpl.Configuration;
using webapitmpl.Utility;

namespace webapitmpl
{
    class Program
    {
        private static void Main(string[] args)
        {
            var cfg = ServiceConfiguration.GetCurrent();

            StartOptions options = new StartOptions();
            cfg.Configure(options);

            using (WebApp.Start(
                options,
                app =>
                {
                    var startup = new DelegatingServerStartup()
                    {
                        DelegatingServer = cfg
                    };

                    startup.Configuration(app);
                }))
            {
                WaitForConsoleCancel();
            }
        }

        private static void WaitForConsoleCancel()
        {
            var complete = new ManualResetEvent(false);
            Console.CancelKeyPress += new ConsoleCancelEventHandler(
                (sender, e) =>
                {
                    e.Cancel = true;
                    complete.Set();
                });
            complete.WaitOne();
        }
    }
}
