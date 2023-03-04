using System;
using System.Collections.Generic;

namespace Atlas
{
    internal class Settings
    {
        public string rootBackupDir = "E:\\_BACKUPS";

        public List<string> dirsToBackup = new List<string>() {
            "E:\\Desktop",
            "E:\\Documents\\_My Stuff",
            "E:\\Downloads",
            "E:\\Music",
            "E:\\Videos",
            "E:\\Pictures"
        };

        public bool doEncrypt = true;
        public String encryptionPassword = "password";
    }
}