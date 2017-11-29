using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM7Backup
{
    public class BackupArgs
    {
        public String SaveLocation { get; set; }
        public String BackupLocation { get; set; }

        public BackupArgs(String saveLocation, String backupLocation)
        {
            SaveLocation = saveLocation;
            BackupLocation = backupLocation;
        }
    }
}
