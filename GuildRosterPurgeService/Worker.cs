using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace NeverwinterHelper.GuildRosterPurgeService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly RosterPurgeOptions _options;
        private readonly MemberRoster _memberRoster;
        private readonly PurgeRoster _purgeRoster;
        private readonly IHostApplicationLifetime _hostLifetime;

        public Worker(ILogger<Worker> logger, IOptions<RosterPurgeOptions> options, MemberRoster memberRoster, PurgeRoster purgeRoster, IHostApplicationLifetime hostLifetime)
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _logger = logger;
            _options = options.Value;
            _memberRoster = memberRoster;
            _purgeRoster = purgeRoster;
            _hostLifetime = hostLifetime;
        }

       
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string inputFilename = string.Empty;
            string outputFilename = string.Empty;
            DateTime cutoffDate;

            try
            {
                if (TryValidateInputFile(_options.InputFile, out inputFilename) && TryGenerateOutputFilename(_options.OutputFile,inputFilename, out outputFilename) && TryGenerateCutoffDate(_options.RosterDate, out cutoffDate))
                {
                    Dictionary<string, DateTime> lastActiveRoster = await _memberRoster.GenerateLastActiveRoster(inputFilename).ConfigureAwait(false);

                    int purgeCount = await _purgeRoster.GeneratePurgeRoster(lastActiveRoster, outputFilename, cutoffDate).ConfigureAwait(false);
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                    _logger.LogInformation(Constants.PurgeCountMessage, purgeCount);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                }
                else
                {
                    //todo: display help for command line options
                }
            }
            catch (Exception ex)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                _logger.LogError(ex, Constants.MethodExceptionText, nameof(ExecuteAsync));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                throw;
            }
            finally
            {
                _hostLifetime.StopApplication();
            }
        }
        private string GenerateOutputFilename(string inputFilename)
        {
            string returnValue = string.Empty;

            try
            {
                returnValue = Path.Combine(Path.GetDirectoryName(inputFilename), Path.GetFileNameWithoutExtension(inputFilename) + "_PurgeList.txt");
            }
            catch (Exception ex)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                _logger.LogError(ex, Constants.MethodExceptionParmText, nameof(GenerateOutputFilename), inputFilename);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                throw;
            }

            return returnValue;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "This method never throws an exception by design. it always returns a bool.")]
        private bool TryGenerateCutoffDate(string rosterDate, out DateTime cutoffDate)
        {
            bool returnValue = false;
            cutoffDate = DateTime.Now.AddMonths(-1);

            try
            {
                if (!string.IsNullOrWhiteSpace(rosterDate))
                {
                    if (rosterDate.Length == 8)
                    {
                        DateTime rDate;
                        if (DateTime.TryParse($"{rosterDate.Substring(4, 2)}/{rosterDate.Substring(6, 2)}/{rosterDate.Substring(0, 4)}", out rDate))
                        {
                            cutoffDate = rDate.AddMonths(-1);
                            returnValue = true;

                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException(nameof(rosterDate), $"rosterDate is not a valid YYYYMMDD value({rosterDate})", rosterDate);
                        }

                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException(nameof(rosterDate), $"rosterDate is not a valid YYYYMMDD value({rosterDate}). Must be 8 digits.", rosterDate);
                    }
                }
            }
            catch (Exception ex)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                _logger.LogError(ex, Constants.MethodExceptionParmText, nameof(TryGenerateCutoffDate),rosterDate);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                returnValue = false;
            }
            return returnValue;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "This method never throws an exception by design. it always returns a bool.")]
        private bool TryValidateInputFile(string inputFilename, out string fileName)
        {
            bool returnValue = false;
            fileName = string.Empty; 
            try
            {
                if (string.IsNullOrWhiteSpace(inputFilename))
                {
                    throw new ArgumentNullException(nameof(inputFilename));
                }
                else if (!File.Exists(inputFilename))
                {
                    throw new FileNotFoundException(inputFilename);
                }
                else
                {
                    fileName = inputFilename;
                    returnValue = true;
                }

            }
            catch (Exception ex)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                _logger.LogError(ex, Constants.MethodExceptionParmText, nameof(TryValidateInputFile), inputFilename);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                returnValue = false;
            }
            return returnValue;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "This method never throws an exception by design. it always returns a bool.")]
        private bool TryGenerateOutputFilename(string outputFilenameParameter,string inputFilename, out string fileName)
        {
            bool returnValue = false;
            fileName = string.Empty;
            try
            {
                if (string.IsNullOrWhiteSpace(outputFilenameParameter))
                {
                    fileName = GenerateOutputFilename(inputFilename);
                }
                else
                {
                    fileName = inputFilename;
                }
                
                if (File.Exists(fileName))
                {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                    _logger.LogError("Output file ({outputFile}) already exists.", fileName);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                    throw new FileExistsException("Output file exists.", nameof(fileName));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                }
                else
                {
                    returnValue = true;
                }

            }
            catch (Exception ex)
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                _logger.LogError(ex, Constants.MethodExceptionText, nameof(TryGenerateOutputFilename));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                returnValue = false;
            }
            return returnValue;
        }
    }
}
