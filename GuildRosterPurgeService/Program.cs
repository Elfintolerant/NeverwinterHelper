using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NeverwinterHelper.GuildRosterPurgeWorker;
using Serilog;

namespace NeverwinterHelper.GuildRosterPurgeService
{
    public static class Program
    {
        //todo: add debug level logging
        public static void Main(string[] args)
        {
            var result = Parser.Default.ParseArguments<CommandLineOptions>(args)
                .WithParsed<CommandLineOptions>(Options =>
                    CreateHostBuilder(BuildConfiguration(Options)).Build().Run());
        }

        private static string[] BuildConfiguration(CommandLineOptions commandLineOptions)
        {
            List<string> returnValue = new List<string>
            {
                "--RosterPurge:InputFile",
                commandLineOptions.InputFile,

                "--RosterPurge:RosterDate",
                commandLineOptions.RosterDate,

                "--RosterPurge:InactiveMonths",
                commandLineOptions.InactiveMonths.ToString(CultureInfo.InvariantCulture)
            };

            if (!string.IsNullOrWhiteSpace(commandLineOptions.OutputFile))
            {
                returnValue.Add("--RosterPurge:OutputFile");
                returnValue.Add(commandLineOptions.OutputFile);
            }

            return returnValue.ToArray();
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
