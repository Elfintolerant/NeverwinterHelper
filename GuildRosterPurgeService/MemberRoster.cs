using CsvHelper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NeverwinterHelper.GuildRosterPurgeService
{
    public class MemberRoster : IDisposable
    {
        private ILogger<MemberRoster> _logger;
        private bool disposedValue;

        public MemberRoster(ILogger<MemberRoster> logger)
        {
            _logger = logger;
        }

        public async Task<Dictionary<string, DateTime>> GenerateLastActiveRoster(string inputFilename)
        {
            Dictionary<string, DateTime> lastActiveRoster = new System.Collections.Generic.Dictionary<string, DateTime>();

            try
            {
                IAsyncEnumerable<GuildRosterLayout> guildRoster;
                var reader = new StreamReader(inputFilename);
                using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    DateTime lastActiveDate = DateTime.MinValue;
                    csvReader.Configuration.BadDataFound = null;
                    guildRoster = csvReader.GetRecordsAsync<GuildRosterLayout>();
                    await foreach (GuildRosterLayout member in guildRoster)
                    {
                        if (DateTime.TryParse(member.LastActiveDate, out lastActiveDate))
                        {
                            if (lastActiveRoster.ContainsKey(member.AccountHandle))
                            {
                                if (lastActiveDate.Date > lastActiveRoster[member.AccountHandle].Date)
                                {
                                    lastActiveRoster[member.AccountHandle] = lastActiveDate;
                                }
                            }
                            else
                            {
                                lastActiveRoster.Add(member.AccountHandle, lastActiveDate);
                            }
                        }
                        else
                        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                            _logger.LogWarning("Last Active Date ({lastActiveDate}) for {characterName}{accountHandle} is invalid.", member.LastActiveDate, member.CharacterName, member.AccountHandle);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                        }
                    }
                }
            }
            catch (Exception ex)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                _logger.LogError(ex, Constants.MethodExceptionParmText, inputFilename);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                throw;
            }
            return lastActiveRoster;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    this._logger = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~MemberRoster()
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
