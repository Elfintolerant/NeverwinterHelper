using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NeverwinterHelper.GuildRosterPurgeWorker
{
    public class FileExistsException : Exception
    {
        public FileExistsException()
        {
        }
        public FileExistsException(string message) : base(message)
        {
        }

        public FileExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FileExistsException(string message, string filename) : base(message)
        {
            Filename = filename;
        }

        public FileExistsException(string message, Exception innerException, string filename) : base(message, innerException)
        {
            Filename = filename;
        }

        public string Filename { get; set; }
    
    }
}
