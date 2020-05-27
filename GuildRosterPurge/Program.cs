using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;

namespace NeverwinterHelper.GuildRosterPurge
{
    /// <summary>
    /// main entry point for console app
    /// </summary>
    class Program
    {
        /// <summary>
        /// static entry point Main
        /// </summary>
        /// <param name="args">array of string command line parameters</param>
        static void Main(string[] args)
        {
            try
            {
                var services = new ServiceCollection();
                ConfigureServices(services);

                using (ServiceProvider serviceProvider = services.BuildServiceProvider())
                {
                    ApplicationController app = serviceProvider.GetService<ApplicationController>();
                    app.Run(args);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception Occured::{ex.Source}::{ex.Message}");
                Console.WriteLine($":::::{ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Hit ENTER to continue.");
                Console.ReadLine();
            }
        }

        private static void ConfigureServices(ServiceCollection services)
        {

            var serilogLogger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateLogger();

            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Information);
                builder.AddSerilog(logger: serilogLogger, dispose: true);
            });

            services.AddTransient<RosterParser>();
            services.AddTransient<ApplicationController>();
        }
    }

}
