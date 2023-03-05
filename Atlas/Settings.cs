using System;
using System.Collections.Generic;

namespace Atlas
{
    internal class Settings
    {
        public String rootBackupDir = "E:\\_BACKUPS";

        public List<string> dirsToBackup = new List<string>()
        {
            "E:\\Desktop",
            "E:\\Documents\\_My Stuff",
            "E:\\Downloads",
            "E:\\Music",
            "E:\\Videos",
            "E:\\Pictures"
        };

        public bool doEncrypt = true;
        public String encryptionPassword = "password";

        public String serverAddress = "http://localhost/atlas";

        public dynamic serverEndpoints = new
        {
            getCommand = "/api/get_command.php"
        };
    }
}