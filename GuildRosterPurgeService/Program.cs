using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace NeverwinterHelper.GuildRosterPurgeService
{
    public static class Program
    {
        //todo: add debug level logging
        public static void Main(string[] args)
        {
            //todo: consider looking for -? --help hereZ???
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureLogging(loggingBuilder =>
            {
                var serilogLogger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger();

                loggingBuilder.ClearProviders();
                loggingBuilder.SetMinimumLevel(LogLevel.Information);
                loggingBuilder.AddSerilog(serilogLogger, dispose: true);
            })
            
            .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;

                    services.Configure<RosterPurgeOptions>(configuration.GetSection("RosterPurge"));
                    
                    services.AddTransient<MemberRoster>();
                    services.AddTransient<PurgeRoster>();
                    services.AddHostedService<Worker>();
                });
    }
}
