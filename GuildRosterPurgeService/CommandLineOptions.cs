using CommandLine;
using CommandLine.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeverwinterHelper.GuildRosterPurgeService
{
    internal class CommandLineOptions
    {
        [Option('i', HelpText ="The full path and filename to the inputfile.",Required = true )]
        public string InputFile { get; set; }

        [Option('o',HelpText = "The full path and filename to the outputfile. Optional: will be system generated if not provided.",Required = false)]
        public string OutputFile { get; set; }
        [Option('r',HelpText ="The date the roster file was exported from Neverwinter in YYYYMMDD format; must be 8 characters.  Defaults to current date if not provided.",Required = false)]
        public string RosterDate { get; set; }

        [Option('a',HelpText ="How many months of inactivity to get purged.  Defaults to 1 month if not provided.",Required = false,Default = 1)]
        public int InactiveMonths { get; set; }

        [Usage(ApplicationAlias = "GuildRosterPurge")]
        public static IEnumerable<Example> Examples
        {
            get
            {
                return new List<Example>() {
                    new Example("Create a purge file of all the inactive guild members.", new CommandLineOptions { InputFile = @"E:\\GBD\Rosters\ROSTER_20200518.csv", RosterDate = "20200518" })
                };
            }
        }
    }
}
