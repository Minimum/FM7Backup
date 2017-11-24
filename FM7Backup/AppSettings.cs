using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM7Backup
{
    public class AppSettings
    {
        public String SaveFileLocation { get; set; }
        public String BackupLocation { get; set; }
        public DateType DateType { get; set; }

        public AppSettings()
        {
            SaveFileLocation = "";
            BackupLocation = "";
            DateType = DateType.Readable;
        }
    }

    public enum DateType
    {
        Readable = 0,
        Unix
    }
}
