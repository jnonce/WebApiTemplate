using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;

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
            StartOptions options = new StartOptions()
            {
                Port = 8999
            };

            using (WebApp.Start<Startup>(options))
            {
                await OnComplete();
                await Task.Delay(1000);
            }

            await Task.Delay(1000);
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
