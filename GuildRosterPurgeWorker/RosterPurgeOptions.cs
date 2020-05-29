using System;
using System.Collections.Generic;
using System.Text;

namespace NeverwinterHelper.GuildRosterPurgeWorker
{
    public class RosterPurgeOptions
    {
        public string InputFile { get; set; }
        public string OutputFile { get; set; }
        public string RosterDate { get; set; }
        public int InactiveMonths { get; set; }
    }
}
