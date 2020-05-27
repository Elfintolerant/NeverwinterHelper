using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace NeverwinterHelper.GuildRosterPurge
{
    class CommandLineParameters
    {
        [Option('i',"input",HelpText ="Full path and filename of input file is required.",Required =true )]
        public string inputFile { get; set; }

        [Option('o', "output", HelpText = "Full path and filename of output file is optional.", Required = false)]
        public string outputFile { get; set; }
        [Option('r',"rosterdate", HelpText ="date the roster was generated as YYYYMMDD", Required = false)]
        public string rosterdate { get; set; }
    }
}
