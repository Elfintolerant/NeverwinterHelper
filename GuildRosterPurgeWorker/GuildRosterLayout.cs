using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace NeverwinterHelper.GuildRosterPurgeWorker
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses", Justification = "Late bound")]
    internal class GuildRosterLayout
    {
        [Name("Character Name")]
        public string CharacterName { get; set; }
        [Name("Account Handle")]
        public string AccountHandle { get; set; }
        [Name("Level")]
        public int Level { get; set; }
        [Name("Class")]
        public string Class { get; set; }
        [Name("Guild Rank")]
        public string GuildRank { get; set; }
        [Name("Contribution Total")]
        public int ContributionTotal { get; set; }
        [Name("Join Date")]
        public string JoinDate { get; set; }
        [Name("Rank Change Date")]
        public string RankChangeDate { get; set; }
        [Name("Last Active Date")]
        public string LastActiveDate { get; set; }
        [Name("Status")]
        public string Status { get; set; }
        [Name("Public Comment")]
        public string PublicComment { get; set; }
        [Name("Public Comment Last Edit Date")]
        public string PublicCommentLastEditDate { get; set; }
    }
}
