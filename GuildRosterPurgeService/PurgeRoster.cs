using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NeverwinterHelper.GuildRosterPurgeService
{
    public class PurgeRoster : IDisposable
    {
        private ILogger<PurgeRoster> _logger;
        private bool disposedValue;

        public PurgeRoster(ILogger<PurgeRoster> logger)
        {
            _logger = logger;
        }

        public async Task<int> GeneratePurgeRoster(Dictionary<string, DateTime> lastActiveRoster,string outputFilename, DateTime cutoffDate)
        {
            int returnValue = 0;
            SortedDictionary<string, DateTime> purgeRoster = new SortedDictionary<string, DateTime>();

            if (lastActiveRoster is null)
            {
                throw new ArgumentNullException(nameof(lastActiveRoster));
            }

            try
            {
                await Task.Run(() => PopulatePurgeRoster(lastActiveRoster, cutoffDate, purgeRoster)).ConfigureAwait(false);
                await Task.Run(() => CreatePurgeRosterFile(purgeRoster, outputFilename, cutoffDate)).ConfigureAwait(false);
                returnValue = purgeRoster.Count;
            }
            catch (Exception ex)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                _logger.LogError(ex, Constants.MethodExceptionText,nameof(GeneratePurgeRoster));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                throw;

            }

            return returnValue;
        }
        private void CreatePurgeRosterFile(SortedDictionary<string, DateTime> purgeRoster, string outputFileName, DateTime purgeCutoffDate)
        {
            try
            {
                using (System.IO.StreamWriter sw = new StreamWriter(outputFileName, false))
                {
                    sw.WriteLine($"Purge Cutoff Date: {purgeCutoffDate.ToShortDateString()}");
                    sw.WriteLine();
                    foreach (KeyValuePair<string, DateTime> member in purgeRoster)
                    {
                        sw.WriteLine($"{member.Key} , {member.Value.ToShortDateString()}");
                    }
                }
            }
            catch (Exception ex)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                _logger.LogError(ex, Constants.MethodExceptionText,nameof(CreatePurgeRosterFile));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                throw;
            }
        }
        private static void PopulatePurgeRoster(Dictionary<string, DateTime> lastActiveRoster, DateTime purgeCutoffDate, SortedDictionary<string, DateTime> purgeRoster)
        {
            foreach (KeyValuePair<string, DateTime> member in lastActiveRoster)
            {
                if (member.Value.Date <= purgeCutoffDate.Date)
                {
                    purgeRoster.Add(member.Key, member.Value);
                }
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _logger = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PurgeRoster()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
