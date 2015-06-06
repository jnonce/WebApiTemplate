using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            using (WebApp.Start < webapitmpl.App_Start.Startup>(options))
            {
                Log("Server listening for connections");
                await OnComplete();

                Log("Shutdown registered");
            }

            await Task.Delay(1500);
        }

        private static void Log(string p)
        {
            Console.WriteLine(p);
        }

        private static Task OnComplete()
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
