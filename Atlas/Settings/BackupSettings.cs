using System;
using System.Collections.Generic;

namespace Atlas
{
    internal class BackupSettings
    {
        public String rootBackupDir = "E:\\_BACKUPS";

        public List<string> dirsToBackup = new List<string>()
        {
            //"E:\\Desktop",
            "E:\\Documents\\_My Stuff\\Music Production\\Kits",
            //"E:\\Downloads",
            //"E:\\Music",
            //"E:\\Videos",
            "E:\\Pictures"
        };

        public bool doEncrypt = true;
        public String encryptionPassword = "password";
    }
}