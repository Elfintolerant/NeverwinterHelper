using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace NeverwinterHelper.GuildRosterPurge
{
    internal class RosterParser : IDisposable
    {
        private ILogger<ApplicationController> _logger;
        private string _inputFile;
        private string _outputFile;
        private DateTime _purgeCutoffDate = DateTime.Now.AddMonths(-1);
        private System.Collections.Generic.Dictionary<string, DateTime> _lastActiveRoster = new System.Collections.Generic.Dictionary<string, DateTime>();
        private SortedDictionary<string, DateTime> _purgeRoster = new SortedDictionary<string, DateTime>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public RosterParser(ILogger<ApplicationController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// GeneratePurgeFile creates the list of members to purge in a file.
        /// </summary>
        public void GeneratePurgeFile(string inputFile, string outputFile, DateTime? rosterDate)
        {
            ValidateParameters(inputFile, outputFile, rosterDate);
            PopulateLastActiveRoster();
            PopulatePurgeRoster();
            CreateOutputFile();
         }

        private void ValidateParameters(string inputFile, string outputFile, DateTime? rosterDate)
        {
            if (string.IsNullOrEmpty(inputFile))
            {
                throw new ArgumentNullException("inputFile");
            }
            if (!File.Exists(inputFile))
            {
                throw new ArgumentException("inputFile", new FileNotFoundException(inputFile));
            }
            if (string.IsNullOrEmpty(outputFile))
            {
                throw new ArgumentNullException("outputFile");
            }
            if (File.Exists(outputFile))
            {
                throw new ArgumentException("outputFile", new ArgumentOutOfRangeException(inputFile, $"Outfile file ({outputFile}) exists."));
            }
            if (rosterDate.HasValue)
            {
                this._purgeCutoffDate = rosterDate.Value.AddMonths(-1);
            }
            this._inputFile = inputFile;
            this._outputFile = outputFile;
        }

        /// <summary>
        /// creates the purge roster output file with minimal formatting.
        /// </summary>
        private void CreateOutputFile()
        {
            using (System.IO.StreamWriter sw = new StreamWriter(_outputFile, false))
            {
                sw.WriteLine($"Purge Cutoff Date: {_purgeCutoffDate.ToShortDateString()}");
                sw.WriteLine();
                foreach (KeyValuePair<string, DateTime> member in _purgeRoster)
                {
                    sw.WriteLine($"{member.Key} , {member.Value.ToShortDateString()}");
                }
            }
        }

        /// <summary>
        /// populates the purge roster when the last active date <= purge cutoff date
        /// </summary>
        private void PopulatePurgeRoster()
        {

            foreach (KeyValuePair<string, DateTime> member in _lastActiveRoster)
            {
                if (member.Value.Date <= _purgeCutoffDate.Date)
                {
                    _purgeRoster.Add(member.Key, member.Value);
                }
            }
        }

        /// <summary>
        /// This creates a last active roster by grouping on the account name and grabbing the most recent last active date
        /// </summary>
        private void PopulateLastActiveRoster()
        {
            IEnumerable<GuildRosterLayout> guildRoster;
            var reader = new StreamReader(_inputFile);
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                DateTime lastActiveDate = DateTime.MinValue;
                csvReader.Configuration.BadDataFound = null;
                guildRoster = csvReader.GetRecords<GuildRosterLayout>();
                foreach (GuildRosterLayout member in guildRoster)
                {
                    if (DateTime.TryParse(member.LastActiveDate, out lastActiveDate))
                    {
                        if (_lastActiveRoster.ContainsKey(member.AccountHandle))
                        {
                            if (lastActiveDate.Date > _lastActiveRoster[member.AccountHandle].Date)
                            {
                                _lastActiveRoster[member.AccountHandle] = lastActiveDate;
                            }
                        }
                        else
                        {
                            _lastActiveRoster.Add(member.AccountHandle, lastActiveDate);
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Last Active Date ({lastActiveDate}) for {characterName}{accountHandle} is invalid.", member.LastActiveDate, member.CharacterName, member.AccountHandle);
                    }
                }
            }

        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _lastActiveRoster.Clear();
                    _purgeRoster.Clear();
                    _logger = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RosterParser()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}