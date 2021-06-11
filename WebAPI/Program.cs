using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Core.Extensions;

namespace WebAPI
{
    /// <summary>
    ///
    /// </summary>
    public static class Program
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        public static async Task Main(string[] args)
        {

            await CreateHostBuilder(args).Build().RunAsync();

        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
                Host.CreateDefaultBuilder(args)
                    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.ConfigureKestrel(options => options.AddServerHeader = false);
                        webBuilder.UseStartup<Startup>();
                    })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Trace);
        });
    }
}