using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using CommandLine;
using System.IO;
using Microsoft.Extensions.Logging;

namespace NeverwinterHelper.GuildRosterPurge
{
    internal class ApplicationController : IDisposable
    {
        private string _inputFile;
        private string _outputFile;
        private DateTime? _rosterDate = null;
        private ILogger<ApplicationController> _logger;
        private RosterParser _rosterParser;
        private bool disposedValue;

        public ApplicationController(ILogger<ApplicationController> logger, RosterParser rosterParser)
        {
            _logger = logger;
            _rosterParser = rosterParser;
        }
        /// <summary>
        /// Run method executes the purge roster creation process
        /// </summary>
        /// <param name="args">string array of commandline arguments</param>
        internal void Run(string[] args)
        {
            if (ValidateCommandlineParameters(args))
            {
                _rosterParser.GeneratePurgeFile(_inputFile, _outputFile, _rosterDate);
            }
            else
            {
                _logger.LogInformation("Commandline parameters failed validation. PurgeRoster creation aborted.");
            }
        }

        private bool ValidateCommandlineParameters(string[] args)
        {
            bool returnValue = false;
            try
            {
                Parser.Default.ParseArguments<CommandLineParameters>(args)
                        .WithParsed<CommandLineParameters>(p =>
                        {
                            if (ValidateInputFile(p.inputFile, out _inputFile))
                            {
                                if (ValidateOutputFile(p.outputFile, _inputFile, out _outputFile))
                                {
                                    returnValue = ValidateRosterDate(p.rosterdate, out _rosterDate);
                                }
                            }
                        });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error");
                returnValue = false;
            }
            return returnValue;
        }

        private bool ValidateRosterDate(string rosterdateParameter, out DateTime? rosterDate)
        {
            bool returnValue = false;
            rosterDate = null;
            try
            { 
                if (string.IsNullOrWhiteSpace(rosterdateParameter))
                {
                    returnValue = true;
                }
                else
                {
                    if (rosterdateParameter.Length == 8)
                    {
                        DateTime rDate;
                        if (DateTime.TryParse($"{rosterdateParameter.Substring(4, 2)}/{rosterdateParameter.Substring(6, 2)}/{rosterdateParameter.Substring(0, 4)}", out rDate))
                        {
                            returnValue = true;
                            rosterDate = new DateTime?(rDate);

                        }
                        else
                        {
                            _logger.LogError("rosterdate parameter is not a valid YYYYMMDD value({rosterDate})", rosterdateParameter);
                        }

                    }
                    else
                    {
                        _logger.LogError("rosterdate parameter is not a valid YYYYMMDD value({rosterDate}). Must be 8 digits.", rosterdateParameter);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error");
                returnValue = false;
            }
           return returnValue;
        }

        private bool ValidateOutputFile(string outputFileParameter, string inputFile,out string outputFile)
        {
            bool returnValue = false;
            outputFile = string.Empty;
            try
            { 
                if (string.IsNullOrWhiteSpace(outputFileParameter))
                {
                    outputFile = Path.Combine(Path.GetDirectoryName(inputFile), Path.GetFileNameWithoutExtension(inputFile) + "_PurgeList.txt");
                }
                else
                {
                    outputFile = outputFileParameter;
                }
                if (File.Exists(outputFile))
                {
                    _logger.LogError("Output file ({outputFile}) already exists.", outputFile);
                }
                else
                {
                    returnValue = true;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error");
                returnValue = false;
            }

            return returnValue;
        }

        private bool ValidateInputFile(string inputFileParameter, out string inputFile)
        {
            bool returnValue = false;
            inputFile = string.Empty;

            try 
            { 
                if (string.IsNullOrWhiteSpace(inputFileParameter))
                {
                    _logger.LogError("inputFile parameter is required. It cannot be empty or whitespace.");
                }
                else if (!File.Exists(inputFileParameter))
                {
                    _logger.LogError("Input file ({inputFile}) does not exists.", inputFileParameter);
                }
                else
                {
                    inputFile = inputFileParameter;
                    returnValue = true;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected Error");
                returnValue = false;
            }
           return returnValue;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _rosterParser = null;
                    _logger = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ApplicationController()
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
