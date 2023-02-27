using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas
{
    internal class Settings
    {
        public string Root_Backup_Dir = "E:\\_BACKUPS";
        public List<string> Backup_Dirs = new List<string>() {
            //"E:\\Desktop",
            //"E:\\Documents",
            //"E:\\Downloads",
            "E:\\Music",
            //"E:\\Videos",
            //"E:\\Pictures"
        };

        public bool Encrypt = true;
        public String EncryptionPassword = "password"; 
    }
}
